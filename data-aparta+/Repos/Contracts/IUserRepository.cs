using data_aparta_.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Contracts
{
    public interface IUserRepository
    {

        Task CreateUserAsync(RegisterInput register, Guid id);
        Task<UserResponse> GetUserByCognitoIdAsync(string userId);

    }
}
