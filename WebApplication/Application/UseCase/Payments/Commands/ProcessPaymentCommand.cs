using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Payments.Commands
{
    public class ProcessPaymentCommand
    {
        public Guid ReservationId { get; set; }
        public int UserId { get; set; }
    }
}
