using Amazon.S3.Model;
using data_aparta_.DTOs;
using data_aparta_.Repos.Auth;
using data_aparta_.Repos.Contracts;

namespace aparta_.Types
{
    [MutationType]
    public class AuthMutations
{
        /*
        private readonly IAuthRepository authRepository;
        private readonly IUserRepository userRepository;

        public AuthMutations(IAuthRepository repo, IUserRepository userRepository)
        {
            authRepository = repo;
            this.userRepository = userRepository;
        }
        */

        public async Task<string> RegisterUser(RegisterInput input, 
            [Service] IAuthRepository authRepository,
            [Service] IUserRepository userRepository)
        {
            try
            {
                string id = await authRepository.RegisterUserAsync(input);
                await userRepository.CreateUserAsync(input, Guid.Parse(id));
                return "User registered successfully. Please confirm your email";
            }
            catch (Exception ex)
            {
                throw new GraphQLException(ex.Message);
            }

        }

        public async Task<string> ConfirmUser(ConfirmInput input, [Service] IAuthRepository authRepository)
        {
            await authRepository.ConfirmUserAsync(input);
            return "User confirmed successfully";
        }

        public async Task<string> LoginUser(LoginInput input, [Service] IAuthRepository authRepository)
        {
            return await authRepository.LoginUserAsync(input);
        }
        public async Task<string> ForgotPassword(string email, [Service] IAuthRepository authRepository)
        {
            await authRepository.ForgotPasswordAsync(email);
            return "Password reset email sent successfully";
        }
        public async Task<string> ConfirmForgotPassword(ChangePasswordInput input, [Service] IAuthRepository authRepository)
        {
            await authRepository.ConfirmForgotPasswordAsync(input);
            return "Password changed successfully";
        }

        public async Task<UserResponse> GetUserByToken(string token, [Service] IUserRepository userRepository)
        {
            return await userRepository.GetUserByJWT(token);
        }
        /*
        public async Task<UpdateEmailResponse> UpdateEmail(string newEmail, string accessToken)
        {
            return await authRepository.UpdateEmailAsync(newEmail, accessToken);
        }
        public async Task<bool> ConfirmEmailUpdate(string confirmationCode, string accessToken)
        {
            return await authRepository.ConfirmEmailUpdateAsync(confirmationCode, accessToken);
        }*/
        public async Task<UserResponse> ChangeUserInfo(ChangeUserInfoInput input, [Service] IUserRepository userRepository)
        {
            return await userRepository.ChangeUserInfoAsync(input);
        }

    }

}
