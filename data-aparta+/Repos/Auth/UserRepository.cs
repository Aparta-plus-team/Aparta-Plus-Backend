using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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


        public async Task<UserResponse> GetUserByJWT(string jwt)
        {

            try
            {
                var handler = new JwtSecurityTokenHandler();

                // Lee el token sin validar la firma
                var jsonToken = handler.ReadToken(jwt) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    throw new Exception("Token inválido");
                }

                // Obtiene el claim 'sub' que contiene el ID del usuario
                var userId = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("ID de usuario no encontrado en el token");
                }

                return await GetUserByCognitoIdAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar el token: {ex.Message}");
            }

            return new UserResponse
            {

            };
        }

        public async Task<UserResponse> ChangeUserInfoAsync(ChangeUserInfoInput changeUserInfoInput)
        {
            Usuario user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Usuarioid.ToString() == changeUserInfoInput.UserId);

            if (user == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            user.Usuarionombre = changeUserInfoInput.Name;
            user.Usuariotelefono = changeUserInfoInput.PhoneNumber;

            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Usuarioid,
                Email = user.Usuariocorreo,
                Username = user.Usuarionombre
            };
        }


    }
}
