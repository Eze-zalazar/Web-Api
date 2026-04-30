using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Venue { get; set; }
        public DateTime EventDate { get; set; }
        public string Status { get; set; }
        /// Sectores del evento con nombre y precio.
        /// Permite al frontend mostrar información contextual sin depender de las butacas.
        public IEnumerable<SectorInfo> Sectors { get; set; } = Enumerable.Empty<SectorInfo>();
    }

    public class SectorInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}
