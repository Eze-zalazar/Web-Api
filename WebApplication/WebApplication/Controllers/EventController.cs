using Application.UseCase.Eventos.Commands;
using Application.UseCase.Eventos.Handlers;
using Application.UseCase.Eventos.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventController : ControllerBase
    {
        private readonly IGetAllEventsHandler _getAllEventsHandler;
        private readonly IGetEventByIdHandler _getEventByIdHandler;
        private readonly ILogger<EventController> _logger;

        public EventController(
            IGetAllEventsHandler getAllEventsHandler,
            IGetEventByIdHandler getEventByIdHandler,
            ILogger<EventController> logger)
        {
            _getAllEventsHandler = getAllEventsHandler;
            _getEventByIdHandler = getEventByIdHandler;
            _logger = logger;
        }

        // GET api/v1/events?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            //  Validación de parámetros
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { error = "Page y PageSize deben ser mayores a 0." });
            }

            try
            {
                var query = new GetAllEventsQuery { Page = page, PageSize = pageSize };
                var result = await _getAllEventsHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de eventos en la página {Page} con tamaño {PageSize}", page, pageSize);

                // 2. Retornamos el 500, pero ahora sabemos qué pasó detrás de escena.
                return StatusCode(500, new { error = "Ocurrió un error inesperado. Por favor, intente más tarde." });
            }
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
