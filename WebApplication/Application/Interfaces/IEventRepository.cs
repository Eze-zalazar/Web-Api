using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync(int page, int pageSize, string? status = null);

        ///Sin Include — útil cuando solo se necesita verificar existencia del evento
        Task<Event?> GetByIdAsync(int id);

        ///Con Include(Sectors) — para el endpoint de detalle que necesita mostrar sectores
        Task<Event?> GetByIdWithSectorsAsync(int id);
    }
}
