using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/pagos")]
    public class PayController : ControllerBase
    {
        private readonly Application.UseCase.Reservations.Handlers.IPayCommandHandler _payCommandHandler;

        public PayController(Application.UseCase.Reservations.Handlers.IPayCommandHandler payCommandHandler)
        {
            _payCommandHandler = payCommandHandler;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] Application.UseCase.Reservations.Commands.PayCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _payCommandHandler.HandleAsync(command);
                return Ok(new { message = "Pago procesado exitosamente.", status = "Completed" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no encontrada") || ex.Message.Contains("expirad") || ex.Message.Contains("permiso") || ex.Message.Contains("pendiente"))
                {
                    return BadRequest(new { error = ex.Message });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
