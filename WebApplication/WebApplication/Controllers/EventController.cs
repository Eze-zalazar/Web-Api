using Application.UseCase.Eventos.Commands;
using Application.UseCase.Eventos.Handlers;
using Application.UseCase.Eventos.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventController : ControllerBase
    {
        private readonly IGetAllEventsHandler _getAllEventsHandler;
        private readonly IGetEventByIdHandler _getEventByIdHandler;
        private readonly ICreateEventHandler _createEventHandler;
        private readonly ILogger<EventController> _logger;

        public EventController(
            IGetAllEventsHandler getAllEventsHandler,
            IGetEventByIdHandler getEventByIdHandler,
            ICreateEventHandler createEventHandler,
            ILogger<EventController> logger)
        {
            _getAllEventsHandler = getAllEventsHandler;
            _getEventByIdHandler = getEventByIdHandler;
            _createEventHandler = createEventHandler;
            _logger = logger;
        }

        // GET api/v1/events?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Name) || command.Sectors == null || command.Sectors.Count == 0)
            {
                return BadRequest(new { error = "Datos del evento inválidos. Se requiere nombre y al menos un sector." });
            }

            try
            {
                var result = await _createEventHandler.HandleAsync(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { mensaje = "Evento creado exitosamente", eventoId = result.Id });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el evento.");
                return StatusCode(500, new { error = "Ocurrió un error inesperado al crear el evento." });
            }
        }
    }
}
