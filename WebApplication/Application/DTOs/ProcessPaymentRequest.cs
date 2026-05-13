using System;

namespace Application.DTOs
{
    public class ProcessPaymentRequest
    {
        public Guid ReservationId { get; set; }
        // Se puede añadir aquí cardNumber u otros detalles de pago si es necesario.
        public string PaymentMethod { get; set; } = "CreditCard";
    }
}
