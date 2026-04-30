using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface  IAuditLogRepository
    {
        Task AddAsync(Audit_Log auditLog);
        Task<IEnumerable<Audit_Log>> GetAllAsync(int page, int pageSize);
    }
}
