using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation> AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task<Reservation?> GetByIdAsync(Guid id);
        Task AddAsync(Reservation reservation);
        Task<Reservation?> GetByIdAsync(Guid id);
        Task UpdateAsync(Reservation reservation);
        Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(DateTime currentTime);
    }
}
