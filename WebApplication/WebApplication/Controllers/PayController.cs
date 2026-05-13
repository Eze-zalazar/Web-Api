using Application.UseCase.Payments.Commands;
using Application.UseCase.Payments.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/payments")]
    public class PayController : ControllerBase
    {
        private readonly IProcessPaymentHandler _processPaymentHandler;

        public PayController(IProcessPaymentHandler processPaymentHandler)
        {
            _processPaymentHandler = processPaymentHandler;
        }

        // POST api/v1/payments
        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
        {
            try
            {
                var result = await _processPaymentHandler.HandleAsync(command);
                return Ok(new { message = "Pago procesado y reserva completada con éxito.", reservationId = result.Id });
            }
            catch (Exception ex)
            {
                // Un error aquí garantiza que el Rollback se ejecutó en el Handler
                if (ex.Message.Contains("no encontrada") || ex.Message.Contains("no pertenece"))
                {
                    return NotFound(new { error = ex.Message });
                }
                if (ex.Message.Contains("ya ha sido procesada") || ex.Message.Contains("expirado"))
                {
                    return BadRequest(new { error = ex.Message });
                }
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error interno del servidor al procesar el pago.", details = ex.Message });
            }
        }
    }
}
