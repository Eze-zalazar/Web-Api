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
        private readonly ICreateEventCommandHandler _createEventCommandHandler;
        private readonly ILogger<EventController> _logger;

        public EventController(
            IGetAllEventsHandler getAllEventsHandler,
            IGetEventByIdHandler getEventByIdHandler,
            ICreateEventCommandHandler createEventCommandHandler,
            ILogger<EventController> logger)
        {
            _getAllEventsHandler = getAllEventsHandler;
            _getEventByIdHandler = getEventByIdHandler;
            _createEventCommandHandler = createEventCommandHandler;
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

        // POST api/v1/events
        // TODO: Require Admin role here
        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventCommand command)
        {
            try
            {
                var result = await _createEventCommandHandler.HandleAsync(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el evento.");
                return StatusCode(500, new { error = "Ocurrió un error al crear el evento." });
            }
        }

        // POST api/v1/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CrearEventoCommand command, [FromServices] ICreateEventHandler _createEventHandler)
        {
            try
            {
                var result = await _createEventHandler.HandleAsync(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el evento {Nombre}", command.Nombre);
                return StatusCode(500, new { error = "Ocurrió un error inesperado al crear el evento." });
            }
        }
    }
}
