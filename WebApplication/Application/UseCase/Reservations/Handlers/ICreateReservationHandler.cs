using Application.DTOs;
using Application.UseCase.Reservations.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public interface ICreateReservationHandler
    {
        Task<ReservationResponse> HandleAsync(CreateReservationCommand command);
    }
}
