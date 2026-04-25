using Application.UseCase.Seats.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")] // Mantenemos la base de eventos
    public class AsientosController : ControllerBase
    {
        private readonly IGetAllSeatsBySectorHandler _getAllSeatsHandler;

        public AsientosController(IGetAllSeatsBySectorHandler getAllSeatsHandler)
        {
            _getAllSeatsHandler = getAllSeatsHandler;
        }

        // GET api/v1/events/1/seats
        // Es correcto usar {eventId} aquí para que cuelgue del evento
        [HttpGet("{eventId}/seats")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            try
            {
                var query = new GetAllSeatsBySectorQuery { EventId = eventId };
                var result = await _getAllSeatsHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
