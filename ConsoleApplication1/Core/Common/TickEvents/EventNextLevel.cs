using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventNextLevel : SingleTimeEvent
    {
        public EventNextLevel(): base(() => { UiManager.Current.Actions.Append("Explored new level. "); GameManager.Current.GenerateWorld(); })
        {
            TicksRemaining = 1;
        }
    }
}
