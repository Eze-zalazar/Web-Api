using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Commands
{
    public class CreateReservationCommand
    {
        public Guid SeatId { get; set; }
        //public int UserId { get; set; }
        // En esta primera instancia del proyecto solo existe un usuario.
        // El UserId se fija en 1 y no se expone en el body para evitar FK violations.
        public int UserId => 1;
    }
}
