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
        
        //Implementación de métodos para acceder a los eventos
        public async Task<IEnumerable<Event>> GetAllAsync(int page, int pageSize, string? status = null)
        {
            IQueryable<Event> query = _context.Events; // ← IQueryable, no ejecuta aún

            if (!string.IsNullOrEmpty(status))
                query = query.Where(e => e.Status == status); // ← se suma al SQL

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // ← recién acá viaja a la BD
        }
        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
