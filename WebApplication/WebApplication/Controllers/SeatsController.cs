using Application.UseCase.Seats.Handlers;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")] // Mantenemos la base de eventos
    public class SeatsController : ControllerBase
    {
        private readonly IGetAllSeatsBySectorHandler _getAllSeatsHandler;
        private readonly ILogger<SeatsController> _logger;

        public SeatsController(
            IGetAllSeatsBySectorHandler getAllSeatsHandler,
            ILogger<SeatsController> logger)
        {
            _getAllSeatsHandler = getAllSeatsHandler;
            _logger = logger;
        }

        // GET api/v1/events/{id}/seats
        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetByEvent(int id)
        {
            try
            {
                var query = new GetAllSeatsBySectorQuery { EventId = id };
                var result = await _getAllSeatsHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (EventNotFoundException ex)
            {
                // Solo devolvemos 404 para el caso semántico específico de dominio.
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Cualquier otro error (DB caída, null inesperado, etc.) devuelve 500
                // y queda registrado para diagnóstico — no se enmascara como 404.
                _logger.LogError(ex, "Error al obtener butacas del evento {EventId}", id);
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }
    }
}
