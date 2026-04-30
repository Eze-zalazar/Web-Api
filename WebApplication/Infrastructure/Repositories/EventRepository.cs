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
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync(int page, int pageSize, string? status = null)
        {
            IQueryable<Event> query = _context.Events;

            if (!string.IsNullOrEmpty(status))
                query = query.Where(e => e.Status == status);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        
        /// Solo trae el evento, sin sectores.
        /// Usado internamente para verificar existencia (ej: GetAllSeatsBySectorHandler).
       
        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        
        /// Trae el evento con sus sectores cargados.
        /// Usado en el endpoint GET /api/v1/events/{id} para devolver info contextual completa.
        
        public async Task<Event?> GetByIdWithSectorsAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Sectors)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
