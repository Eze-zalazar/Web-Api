using Domain.Entities;
using System.Threading.Tasks;
using Application.UseCase.Usuarios.Commands;

namespace Application.UseCase.Usuarios.Handlers
{
    public interface ILoginHandler
    {
        Task<User> HandleAsync(LoginCommand command);
    }
}
