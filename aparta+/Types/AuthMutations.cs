using data_aparta_.DTOs;
using data_aparta_.Repos.Contracts;

namespace aparta_.Types
{
    [MutationType]
    public class AuthMutations
{
        private readonly IAuthRepository authRepository;
        private readonly IUserRepository userRepository;

        public AuthMutations(IAuthRepository repo, IUserRepository userRepository)
        {
            authRepository = repo;
            this.userRepository = userRepository;
        }

        public async Task<string> RegisterUser(RegisterInput input)
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

        public async Task<string> ConfirmUser(ConfirmInput input)
        {
            await authRepository.ConfirmUserAsync(input);
            return "User confirmed successfully";
        }

        public async Task<string> LoginUser(LoginInput input)
        {
            return await authRepository.LoginUserAsync(input);
        }
        public async Task<string> ForgotPassword(string email)
        {
            await authRepository.ForgotPasswordAsync(email);
            return "Password reset email sent successfully";
        }
        public async Task<string> ConfirmForgotPassword(ChangePasswordInput input)
        {
            await authRepository.ConfirmForgotPasswordAsync(input);
            return "Password changed successfully";
        }

    }

}
