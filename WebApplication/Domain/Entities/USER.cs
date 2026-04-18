using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public IList<Reservation> Reservations { get; set; } = new List<Reservation>();
        public IList<Audit_Log> AuditLogs { get; set; } = new List<Audit_Log>();

    }
}
