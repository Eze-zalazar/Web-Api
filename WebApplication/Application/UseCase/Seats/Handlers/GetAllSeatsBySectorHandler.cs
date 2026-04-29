using Application.DTOs;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.UseCase.Seats.Handlers
{
    public class GetAllSeatsBySectorHandler : IGetAllSeatsBySectorHandler
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IEventRepository _eventRepository;

        public GetAllSeatsBySectorHandler(
            ISeatRepository seatRepository,
            IEventRepository eventRepository)
        {
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<SeatResponse>> HandleAsync(GetAllSeatsBySectorQuery query)
        {
            var evento = await _eventRepository.GetByIdAsync(query.EventId);
            if (evento == null)
                throw new Exception("Evento no encontrado");

            var seats = await _seatRepository.GetAllByEventIdAsync(query.EventId);

            return seats.Select(s => new SeatResponse
            {
                Id = s.Id,
                RowIdentifier = s.RowIdentifier,
                SeatNumber = s.SeatNumber,
                Status = s.Status,
                SectorId = s.SectorId,
                Price = s.Sector.Price
            });
        }
    }
}