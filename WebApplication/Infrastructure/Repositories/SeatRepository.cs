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
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;

        public SeatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>> GetAllByEventIdAsync(int eventId)
        {
            return await _context.Seats
                .AsNoTracking()                          
                .Include(s => s.Sector)                 
                .Where(s => s.Sector.EventId == eventId) 
                .ToListAsync();
        }

        public async Task<Seat?> GetByIdAsync(Guid seatId)
        {
            return await _context.Seats
                .AsNoTracking()                         
                .FirstOrDefaultAsync(s => s.Id == seatId);
        }
        // UpdateAsync NO usa AsNoTracking
        // porque necesita que EF Core rastree el objeto para poder modificarlo
        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat); // necesita tracking

        }
    }
}
