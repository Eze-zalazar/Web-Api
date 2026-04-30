using Application.UseCase.AuditLogs.Handlers;
using Application.UseCase.AuditLogs.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/audit-logs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IGetAllAuditLogsHandler _getAllAuditLogsHandler;
        private readonly ILogger<AuditLogsController> _logger;

        public AuditLogsController(
            IGetAllAuditLogsHandler getAllAuditLogsHandler,
            ILogger<AuditLogsController> logger)
        {
            _getAllAuditLogsHandler = getAllAuditLogsHandler;
            _logger = logger;
        }

        // GET api/v1/audit-logs?page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { error = "Page y PageSize deben ser mayores a 0." });

            try
            {
                var query = new GetAllAuditLogsQuery { Page = page, PageSize = pageSize };
                var result = await _getAllAuditLogsHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los registros de auditoría.");
                return StatusCode(500, new { error = "Ocurrió un error inesperado." });
            }
        }
    }
}
