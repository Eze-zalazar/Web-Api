using Application.UseCase.Reservations.Commands;
using Application.UseCase.Reservations.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly ICreateReservationHandler _createReservationHandler;

        public ReservationController(ICreateReservationHandler createReservationHandler)
        {
            _createReservationHandler = createReservationHandler;
        }

        // POST api/v1/reservations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
        {
            // Valida que SeatId no sea Guid.Empty
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _createReservationHandler.HandleAsync(command);
                return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { error = "Alguien más reservó este asiento en este momento. Intente de nuevo." });
            }
            catch (Exception ex) when (ex.Message.Contains("no encontrada"))
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("no disponible"))
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }
    }
}
