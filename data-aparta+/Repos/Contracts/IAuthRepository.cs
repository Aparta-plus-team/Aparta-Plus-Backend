using data_aparta_.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Contracts
{
    public interface IAuthRepository
    {
        Task<string> RegisterUserAsync (RegisterInput registerRequest);
        Task ConfirmUserAsync(ConfirmInput confirmRequest);
        Task<string> LoginUserAsync(LoginInput loginRequest);
        Task ConfirmForgotPasswordAsync(ChangePasswordInput changePasswordRequest);
        Task ForgotPasswordAsync(string email);
        Task<UpdateEmailResponse> UpdateEmailAsync(string newEmail, string accessToken);
        Task<bool> ConfirmEmailUpdateAsync(string confirmationCode, string accessToken);
        Task<bool> IsUserConfirmedAsync(string email);




    }
}
