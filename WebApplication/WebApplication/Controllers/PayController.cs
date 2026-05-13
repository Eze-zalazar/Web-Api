using Application.DTOs;
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

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            try
            {
                // Asumimos un UserId harcodeado o extraído del token JWT si hubiera autenticación.
                // Por ahora lo simularemos como 1 para cumplir con la firma.
                int userId = 1; 

                var result = await _processPaymentHandler.HandleAsync(request, userId);
                
                if (result)
                {
                    return Ok(new { Message = "Payment processed successfully" });
                }

                return BadRequest(new { Message = "Payment failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
    }
}
