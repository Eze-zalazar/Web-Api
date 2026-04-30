using Application.DTOs;
using Application.Interfaces;
using Application.UseCase.AuditLogs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuditLogs.Handlers
{
    public class GetAllAuditLogsHandler : IGetAllAuditLogsHandler
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public GetAllAuditLogsHandler(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IEnumerable<AuditLogResponse>> HandleAsync(GetAllAuditLogsQuery query)
        {
            var logs = await _auditLogRepository.GetAllAsync(query.Page, query.PageSize);

            return logs.Select(l => new AuditLogResponse
            {
                Id = l.Id,
                UserId = l.UserId,
                Action = l.Action,
                EntityType = l.EntityType,
                EntityId = l.EntityId,
                Details = l.Details,
                CreatedAt = l.CreatedAt
            });
        }
    }
}
