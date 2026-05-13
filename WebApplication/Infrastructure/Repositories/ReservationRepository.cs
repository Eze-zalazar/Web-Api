using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation> AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            return reservation;
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public async Task<IEnumerable<Reservation>> GetExpiredPendingAsync(DateTime currentTime)
        {
            return await _context.Reservations
                .Where(r => r.Status == "Pending" && r.ExpiresAt < currentTime)
                .ToListAsync();
        }

        public Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return Task.CompletedTask;
        }
    }
}
