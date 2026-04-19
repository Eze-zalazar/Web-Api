using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Seats.Handlers
{
    public interface IGetAllSeatsBySectorHandler
    {
        Task<IEnumerable<SeatResponse>> HandleAsync(GetAllSeatsBySectorQuery query);
    }
}
