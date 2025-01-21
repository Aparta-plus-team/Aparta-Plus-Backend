using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly CognitoUserPool _userPool;
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        private readonly string _clientSecret;

        public AuthRepository(IConfiguration configuration, AmazonCognitoIdentityProviderClient client, ApartaPlusContext context)
        {
            _cognitoClient = client;
            _clientSecret = configuration["AWS:ClientSecret"];
            _userPool = new CognitoUserPool(
                configuration["AWS:UserPoolId"],
                configuration["AWS:ClientId"],
                _cognitoClient,
                _clientSecret
            );
        }

        public async Task<string> RegisterUserAsync(RegisterInput registerRequest)
        {
            var attributes = new Dictionary<string, string>
        {
            {"name", registerRequest.Username},
            {"email", registerRequest.Email}
        };

            var signUpRequest = new SignUpRequest
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password,
                ClientId = _userPool.ClientID,
                SecretHash = CalculateSecretHash(registerRequest.Username),
                UserAttributes = attributes.Select(kvp => new AttributeType
                {
                    Name = kvp.Key,
                    Value = kvp.Value
                }).ToList()
            };

            try
            {
                var response = await _cognitoClient.SignUpAsync(signUpRequest);
                Console.WriteLine($"User {registerRequest.Username} registered with ID {response.UserSub}");
                return response.UserSub;

            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error registering user: {ex.Message}");
            }
        }

        private string CalculateSecretHash(string username)
        {
            byte[] message = Encoding.UTF8.GetBytes(username + _userPool.ClientID);
            byte[] key = Encoding.UTF8.GetBytes(_clientSecret);

            using (var hmac = new HMACSHA256(key))
            {
                byte[] hash = hmac.ComputeHash(message);
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<LoginResponse> LoginUserAsync(LoginInput loginRequest)
        {
            try
            {
                // Primero verificamos si el usuario está confirmado
                bool isConfirmed = await IsUserConfirmedAsync(loginRequest.Email);
                if (!isConfirmed)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario no verificado. Por favor, verifique su cuenta con el código enviado a su correo electrónico.",
                        Status = LoginStatus.UnverifiedUser,
                        Token = null
                    };
                }

                var user = new CognitoUser(
                    loginRequest.Email,
                    _userPool.ClientID,
                    _userPool,
                    _cognitoClient,
                    _clientSecret
                );

                try
                {
                    var authRequest = new InitiateSrpAuthRequest()
                    {
                        Password = loginRequest.Password
                    };

                    var authResponse = await user.StartWithSrpAuthAsync(authRequest);

                    return new LoginResponse
                    {
                        Success = true,
                        Message = "Inicio de sesión exitoso",
                        Status = LoginStatus.Success,
                        Token = authResponse.AuthenticationResult.IdToken
                    };
                }
                catch (NotAuthorizedException)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Credenciales incorrectas. Por favor, verifique su email y contraseña.",
                        Status = LoginStatus.InvalidCredentials,
                        Token = null
                    };
                }
                catch (UserNotConfirmedException)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario no verificado. Por favor, verifique su cuenta con el código enviado a su correo electrónico.",
                        Status = LoginStatus.UnverifiedUser,
                        Token = null
                    };
                }
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Error durante el inicio de sesión: {ex.Message}",
                    Status = LoginStatus.Error,
                    Token = null
                };
            }
        }

        public async Task ConfirmUserAsync(ConfirmInput confirmRequest)
        {
            var user = new CognitoUser(
                confirmRequest.Email,
                _userPool.ClientID,
                _userPool,
                _cognitoClient,
                _clientSecret
            );

            try
            {
                await user.ConfirmSignUpAsync(confirmRequest.ConfirmationCode, true);
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error confirming user: {ex.Message}");
            }
        }
        public async Task ForgotPasswordAsync(string email)
        {
            var request = new ForgotPasswordRequest
            {
                ClientId = _userPool.ClientID,
                Username = email,
                SecretHash = CalculateSecretHash(email)
            };

            try
            {
                await _cognitoClient.ForgotPasswordAsync(request);
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error initiating password reset: {ex.Message}");
            }
        }

        public async Task ConfirmForgotPasswordAsync(ChangePasswordInput passwordInput)
        {
            var request = new ConfirmForgotPasswordRequest
            {
                ClientId = _userPool.ClientID,
                Username = passwordInput.Email,
                ConfirmationCode = passwordInput.ConfirmationCode,
                Password = passwordInput.NewPassword,
                SecretHash = CalculateSecretHash(passwordInput.Email)
            };

            try
            {
                await _cognitoClient.ConfirmForgotPasswordAsync(request);
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error confirming password reset: {ex.Message}");
            }
        }

        public async Task<UpdateEmailResponse> UpdateEmailAsync(string newEmail, string accessToken)
        {
            try
            {
                var attributes = new List<AttributeType>
            {
                new AttributeType
                {
                    Name = "email",
                    Value = newEmail
                }
            };

                var updateRequest = new UpdateUserAttributesRequest
                {
                    UserAttributes = attributes,
                    AccessToken = accessToken
                };

                await _cognitoClient.UpdateUserAttributesAsync(updateRequest);

                return new UpdateEmailResponse
                {
                    Success = true,
                    Message = "Se ha enviado un código de verificación al nuevo correo electrónico",
                    DestinationEmail = newEmail
                };
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error updating email: {ex.Message}");
            }
        }

        public async Task<bool> ConfirmEmailUpdateAsync(string confirmationCode, string accessToken)
        {
            try
            {
                var verifyRequest = new VerifyUserAttributeRequest
                {
                    AttributeName = "email",
                    Code = confirmationCode,
                    AccessToken = accessToken
                };

                await _cognitoClient.VerifyUserAttributeAsync(verifyRequest);
                return true;
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error confirming email update: {ex.Message}");
            }
        }

        public async Task<bool> IsUserConfirmedAsync(string email)
        {
            try
            {
                var request = new AdminGetUserRequest
                {
                    UserPoolId = _userPool.PoolID,
                    Username = email,
                };

                try
                {
                    var response = await _cognitoClient.AdminGetUserAsync(request);
                    return response.UserStatus == UserStatusType.CONFIRMED;
                }
                catch (UserNotFoundException)
                {
                    throw new Exception($"Usuario no encontrado: {email}");
                }
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error checking user confirmation status: {ex.Message}");
            }
        }

        public async Task<string> ResendVerificationCodeAsync(string email)
        {
            try
            {
                var request = new ResendConfirmationCodeRequest
                {
                    ClientId = _userPool.ClientID,
                    Username = email,
                    SecretHash = CalculateSecretHash(email)
                };

                await _cognitoClient.ResendConfirmationCodeAsync(request);
                return "Código de verificación reenviado";
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error resending verification code: {ex.Message}");
            }
        }
    }
    }

public class UpdateEmailResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string DestinationEmail { get; set; }
    }
