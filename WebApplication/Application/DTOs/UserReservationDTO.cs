using System;

namespace Application.DTOs
{
    public class UserReservationDTO
    {
        public Guid ReservationId { get; set; }
        public string EventName { get; set; }
        public string EventVenue { get; set; }
        public DateTime EventDate { get; set; }
        public string? EventImageUrl { get; set; }
        public string SectorName { get; set; }
        public int SeatNumber { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
