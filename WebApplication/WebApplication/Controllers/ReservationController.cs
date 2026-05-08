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
        private readonly IGetReservationsByUserHandler _getReservationsByUserHandler;
        private readonly ICancelReservationHandler _cancelReservationHandler;

        public ReservationController(
            ICreateReservationHandler createReservationHandler,
            IGetReservationsByUserHandler getReservationsByUserHandler,
            ICancelReservationHandler cancelReservationHandler)
        {
            _createReservationHandler = createReservationHandler;
            _getReservationsByUserHandler = getReservationsByUserHandler;
            _cancelReservationHandler = cancelReservationHandler;
        }

        // POST api/v1/reservations/{id}/cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                await _cancelReservationHandler.HandleAsync(id);
                return Ok(new { message = "Reserva cancelada y butaca liberada." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET api/v1/reservations/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var result = await _getReservationsByUserHandler.HandleAsync(userId);
            return Ok(result);
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
