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
            var result = await _createReservationHandler.HandleAsync(command);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }
    }
}
