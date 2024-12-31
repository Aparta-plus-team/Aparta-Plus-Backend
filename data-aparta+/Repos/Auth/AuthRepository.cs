using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using data_aparta_.DTOs;
using data_aparta_.Repos.Contracts;
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

        public AuthRepository(IConfiguration configuration, AmazonCognitoIdentityProviderClient client)
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

        public async Task<string> LoginUserAsync(LoginInput loginRequest)
        {
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
                return authResponse.AuthenticationResult.IdToken;
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw new Exception($"Error logging in: {ex.Message}");
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
    }
    }
