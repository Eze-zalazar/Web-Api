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
    }
}
