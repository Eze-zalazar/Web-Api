using Application.Interfaces;
using Application.UseCase.Usuarios.Commands;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.UseCase.Usuarios.Handlers
{
    public class LoginHandler : ILoginHandler
    {
        private readonly IUserRepository _userRepository;

        public LoginHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> HandleAsync(LoginCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);

            if (user == null)
                throw new Exception("El usuario no existe.");

            // Comparación simple del password (sin hashing para el alcance del proyecto)
            if (user.PasswordHash != command.Password)
                throw new UnauthorizedAccessException("Contraseña incorrecta.");

            return user;
        }
    }
}
