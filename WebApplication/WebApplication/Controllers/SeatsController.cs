using Application.UseCase.Seats.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")] // Mantenemos la base de eventos
    public class SeatsController : ControllerBase
    {
        private readonly IGetAllSeatsBySectorHandler _getAllSeatsHandler;

        public SeatsController(IGetAllSeatsBySectorHandler getAllSeatsHandler)
        {
            _getAllSeatsHandler = getAllSeatsHandler;
        }

        // GET api/v1/events/1/seats
        // Es correcto usar {id} aquí para que cuelgue del evento
        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetByEvent(int id)
        {
            try
            {
                var query = new GetAllSeatsBySectorQuery { EventId = id };
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
