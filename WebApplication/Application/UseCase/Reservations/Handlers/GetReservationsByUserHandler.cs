using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public class GetReservationsByUserHandler : IGetReservationsByUserHandler
    {
        private readonly IReservationRepository _reservationRepository;

        public GetReservationsByUserHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<UserReservationDTO>> HandleAsync(int userId)
        {
            var reservations = await _reservationRepository.GetByUserIdAsync(userId);
            
            return reservations.Select(r => new UserReservationDTO
            {
                ReservationId = r.Id,
                EventName = r.Seat.Sector.Event.Name,
                EventVenue = r.Seat.Sector.Event.Venue,
                EventDate = r.Seat.Sector.Event.EventDate,
                EventImageUrl = r.Seat.Sector.Event.ImageUrl,
                SectorName = r.Seat.Sector.Name,
                SeatNumber = r.Seat.SeatNumber,
                Price = r.Seat.Sector.Price,
                Status = r.Status,
                ReservedAt = r.ReservedAt,
                ExpiresAt = r.ExpiresAt
            });
        }
    }
}
