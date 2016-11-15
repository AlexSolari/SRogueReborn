using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventItemDrop<IDropUnitContainer> : SingleTimeEvent 
        where IDropUnitContainer: IUnit, IDropContainer
    {
        public EventItemDrop(IDropUnitContainer unit) : base(() => GameManager.Current.Entities.Add(unit))
            
        {
        }
    }
}
