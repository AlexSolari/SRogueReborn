using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventNextLevel : TickEventBase
    {
        public override Action Event
        {
            get
            {
                UiManager.Current.Actions.Append("Explored new level. ");
                return () => GameManager.Current.GenerateWorld();
            }
        }

        public EventNextLevel()
        {
            TicksRemaining = 1;
        }
    }
}
