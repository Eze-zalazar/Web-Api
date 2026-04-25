using Application.UseCase.Reservations.Commands;
using Application.UseCase.Reservations.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/reservations")]
    public class ReservasController : ControllerBase
    {
        private readonly ICreateReservationHandler _createReservationHandler;

        public ReservasController(ICreateReservationHandler createReservationHandler)
        {
            _createReservationHandler = createReservationHandler;
        }

        // POST api/v1/reservations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
        {
            try
            {
                var result = await _createReservationHandler.HandleAsync(command);
                // 201 Created es el estándar para POST exitosos
                return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
            }
            catch (Exception ex) when (ex.Message.Contains("no encontrada"))
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("no disponible"))
            {
                return Conflict(new { error = ex.Message }); // 409 Conflict
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                // Esto es por si falló la versión del asiento (ACID)
                return Conflict(new { error = "Alguien más reservó este asiento en este momento. Intente de nuevo." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error inesperado." });
            }
        }
    }
}
