using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public abstract class TickEventBase
    {
        public abstract Action Event { get; }
        public virtual Action OnTimeout { get; private set; }
        public virtual int TicksRemaining { get; set; }
        public virtual IEntity Target { get; set; }
    }
}
