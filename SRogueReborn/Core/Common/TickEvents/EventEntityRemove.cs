using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventEntityRemove : SingleTimeEvent
    {
        public EventEntityRemove(IUnit unit) : base(
            () => {
                GameState.Current.CurrentLevel.Entities.Remove(unit);

                if (unit == GameState.Current.Player)
                {
                    UiManager.Current.LoseGame();
                }
            }
        )
        { }
    }
}
