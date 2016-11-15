using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class SingleTimeEvent : TickEventBase
    {
        public virtual bool IsFired { get; protected set; }

        public virtual Action Action { get; protected set; }

        public override Action Event
        {
            get
            {
                return () => {
                    if (!IsFired)
                        Action();

                    IsFired = true;

                };
            }
        }

        public SingleTimeEvent(Action action)
        {
            IsFired = false;
            Action = action;
            TicksRemaining = 1;
        }
    }
}
