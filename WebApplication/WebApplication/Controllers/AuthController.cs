using Application.UseCase.Usuarios.Commands;
using Application.UseCase.Usuarios.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginHandler _loginHandler;

        public AuthController(ILoginHandler loginHandler)
        {
            _loginHandler = loginHandler;
        }

        // POST api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            try
            {
                var user = await _loginHandler.HandleAsync(command);

                return Ok(new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    isAdmin = user.Email == "admin@admin.com"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
