using System;
using System.Collections.Generic;

namespace Application.UseCase.Eventos.Commands
{
    public class CreateEventCommand
    {
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public List<CreateSectorCommand> Sectors { get; set; } = new List<CreateSectorCommand>();
    }

    public class CreateSectorCommand
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}
