using SRogue.Core.Common;
using SRogue.Core.Common.TickEvents;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRogue.Core.Entities.Concrete.Tiles
{
    public class Exit : Tile
    {
        public override void OnStep(IUnit unit)
        {
            if (unit == GameManager.Current.Player)
            {
                GameManager.Current.OnTickEndEvents.Add(new EventNextLevel());
            }
        }
    }
}
