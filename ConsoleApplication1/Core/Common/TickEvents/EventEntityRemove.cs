using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventEntityRemove : TickEventBase
    {
        public override Action Event
        {
            get
            {
                return () => {
                    GameManager.Current.Entities.Remove(Target as IUnit);

                    if (Target == GameManager.Current.Player)
                    {
                        UiManager.Current.LoseGame();
                    }
                };
            }
        }

        public EventEntityRemove(IUnit unit)
        {
            Target = unit;
            TicksRemaining = 1;
        }
    }
}
