using System;

namespace Application.UseCase.Reservations.Commands
{
    public class PayCommand
    {
        public Guid ReservationId { get; set; }
        public int UserId { get; set; }
        // Simulated payment info
        public string CardNumber { get; set; }
    }
}
