using System;
using System.Collections.Generic;

namespace Application.UseCase.Eventos.Commands
{
    public class SectorRequest
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Capacidad { get; set; }
    }

    public class CrearEventoCommand
    {
        public string Nombre { get; set; }
        public DateTime FechaEvento { get; set; }
        public string Lugar { get; set; }
        public List<SectorRequest> Sectores { get; set; } = new List<SectorRequest>();
    }
}
