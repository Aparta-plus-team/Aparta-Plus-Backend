﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class UserResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public Guid Id { get; set; }
    }

    public class ChangeUserInfoInput
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LoginStatus Status { get; set; }
        public string? Token { get; set; }
    }

    public enum LoginStatus
    {
        Success,
        UnverifiedUser,
        InvalidCredentials,
        Error
    }

}
