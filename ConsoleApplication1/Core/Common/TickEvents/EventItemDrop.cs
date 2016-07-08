using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventItemDrop : TickEventBase
    {
        public override Action Event
        {
            get
            {
                return () => {
                    GameManager.Current.Entities.Add(Target as IUnit);
                };
            }
        }

        public EventItemDrop(IDropContainer unit)
        {
            Target = unit as IUnit;
            TicksRemaining = 1;
        }
    }
}
