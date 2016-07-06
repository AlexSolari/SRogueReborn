using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common
{
    public class TickEvents
    {
        public Action Event { get; set; }
        public int TicksRemaining { get; set; }

        public TickEvents(Action Event)
            :this(Event, 1)
        {

        }

        public TickEvents(Action Event, int ticks)
        {
            this.Event = Event;
            this.TicksRemaining = ticks;
        }
    }
}
