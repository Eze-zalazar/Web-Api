using System;

namespace Application.UseCase.Payments.Commands
{
    public class ProcesarPagoCommand
    {
        public Guid ReservaId { get; set; }
        public int UsuarioId { get; set; }
        public decimal MontoPagado { get; set; }
        public string MetodoPago { get; set; }
    }
}
