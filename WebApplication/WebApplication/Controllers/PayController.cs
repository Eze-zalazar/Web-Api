using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/pagos")]
    public class PayController : ControllerBase
    {
        private readonly Application.UseCase.Payments.Handlers.IProcessPaymentHandler _processPaymentHandler;

        public PayController(Application.UseCase.Payments.Handlers.IProcessPaymentHandler processPaymentHandler)
        {
            _processPaymentHandler = processPaymentHandler;
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarPago([FromBody] Application.UseCase.Payments.Commands.ProcesarPagoCommand command)
        {
            try
            {
                var result = await _processPaymentHandler.HandleAsync(command);
                if (result)
                    return Ok(new { message = "Pago procesado exitosamente y reserva confirmada." });
                
                return BadRequest(new { error = "No se pudo procesar el pago." });
            }
            catch (Exception ex)
            {
                // Devolvemos 400 BadRequest para los mensajes de validación (ej. reserva no encontrada, butaca no válida)
                return BadRequest(new { error = ex.Message });
            }
        }
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
