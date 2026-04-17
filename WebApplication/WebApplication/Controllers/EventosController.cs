using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEventos()
        {
            // Aquí puedes implementar la lógica para obtener los eventos desde la base de datos
            return Ok(new { message = "Lista de eventos" });
        }
    }
}
