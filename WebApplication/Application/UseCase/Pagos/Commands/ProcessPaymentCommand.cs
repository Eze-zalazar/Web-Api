using System;

namespace Application.UseCase.Pagos.Commands
{
    public class ProcessPaymentCommand
    {
        public Guid ReservationId { get; set; }
        public int UserId { get; set; }
    }
}
