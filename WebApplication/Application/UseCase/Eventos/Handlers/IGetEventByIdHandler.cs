using Application.DTOs;
using Application.UseCase.Eventos.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public interface IGetEventByIdHandler
    {
        Task<EventResponse?> HandleAsync(GetEventByIdQuery query);
    }
}
