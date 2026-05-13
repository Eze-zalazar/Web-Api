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
        Task<Reservation?> GetByIdWithSeatAsync(Guid id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Reservation>> GetExpiredPendingReservationsAsync(DateTime currentTime);
        Task UpdateAsync(Reservation reservation);
    }
}
