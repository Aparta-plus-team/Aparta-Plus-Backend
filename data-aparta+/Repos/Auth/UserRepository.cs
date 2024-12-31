using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly ApartaPlusContext _context;

        public UserRepository(ApartaPlusContext context) { 
            _context = context;
        }
        public async Task CreateUserAsync(RegisterInput register, Guid id)
        {
            var newUser = new Usuario
            {
                Usuarioid = id,
                Usuarionombre = register.Name,
                Usuariocorreo = register.Email,
                Usuariotelefono = register.PhoneNumber,
               // Usuariocreado = DateTime.Now,
                Estado = true,
            };

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

        }

        public async Task<UserResponse> GetUserByCognitoIdAsync(string userId)
        {
            Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Usuarioid.ToString() == userId);

            UserResponse userResponse = new UserResponse
            {
                Id = user.Usuarioid,
                Email = user.Usuariocorreo,
                Username = user.Usuarionombre
            };

            return userResponse;

        }

    }
}
