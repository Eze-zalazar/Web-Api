using Application.DTOs;
using Application.UseCase.AuditLogs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuditLogs.Handlers
{
    public interface IGetAllAuditLogsHandler
    {
        Task<IEnumerable<AuditLogResponse>> HandleAsync(GetAllAuditLogsQuery query);
    }
}
