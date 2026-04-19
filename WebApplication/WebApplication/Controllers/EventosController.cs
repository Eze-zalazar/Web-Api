using Application.UseCase.Eventos.Commands;
using Application.UseCase.Eventos.Handlers;
using Application.UseCase.Eventos.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventosController : ControllerBase
    {
        private readonly IGetAllEventsHandler _getAllEventsHandler;
        private readonly IGetEventByIdHandler _getEventByIdHandler;

        public EventosController(
            IGetAllEventsHandler getAllEventsHandler,
            IGetEventByIdHandler getEventByIdHandler)
        {
            _getAllEventsHandler = getAllEventsHandler;
            _getEventByIdHandler = getEventByIdHandler;
        }

        // GET api/v1/events?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllEventsQuery query)
        {
            var result = await _getAllEventsHandler.HandleAsync(query);
            return Ok(result);
        }

        // GET api/v1/events/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetEventByIdQuery { EventId = id };
            var result = await _getEventByIdHandler.HandleAsync(query);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
