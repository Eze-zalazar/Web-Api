using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class EventNotFoundException : Exception
    {
        public int EventId { get; }

        public EventNotFoundException(int eventId)
            : base($"El evento con Id {eventId} no fue encontrado.")
        {
            EventId = eventId;
        }
    }
}
