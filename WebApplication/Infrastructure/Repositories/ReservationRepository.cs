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

        public async Task<Reservation?> GetByIdWithSeatAsync(Guid id)
        {
            return await _context.Reservations
                .Include(r => r.Seat)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Include(r => r.Seat)
                    .ThenInclude(s => s.Sector)
                        .ThenInclude(sc => sc.Event)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReservedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetExpiredPendingReservationsAsync(DateTime currentTime)
        {
            return await _context.Reservations
                .Include(r => r.Seat)
                .Where(r => (r.Status == "Pending" || r.Status == "Reserved") && r.ExpiresAt < currentTime)
                .ToListAsync();
        }

        public Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return Task.CompletedTask;
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return Task.CompletedTask;
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations.FindAsync(id);
        }
    }
}
