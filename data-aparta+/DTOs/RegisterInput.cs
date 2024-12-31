using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class RegisterInput
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class ConfirmInput
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class LoginInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordInput
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
