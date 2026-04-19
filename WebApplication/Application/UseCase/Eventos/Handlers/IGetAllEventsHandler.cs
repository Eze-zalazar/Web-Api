using Application.DTOs;
using Application.UseCase.Eventos.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public interface IGetAllEventsHandler
    {
        Task<IEnumerable<EventResponse>> HandleAsync(GetAllEventsQuery query);
    }
}
