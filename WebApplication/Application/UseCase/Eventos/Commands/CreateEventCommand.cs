using System;
using System.Collections.Generic;

namespace Application.UseCase.Eventos.Commands
{
    public class CreateEventCommand
    {
        public int UserId { get; set; } // Identificador del usuario que intenta crear el evento
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public string? ImageUrl { get; set; }
        public List<SectorDTO> Sectors { get; set; } = new List<SectorDTO>();
    }

    public class SectorDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}
