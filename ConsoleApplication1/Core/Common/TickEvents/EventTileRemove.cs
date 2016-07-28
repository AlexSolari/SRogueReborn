using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventTileRemove : TickEventBase
    {
        public override Action Event
        {
            get
            {
                return () => {
                    GameManager.Current.Tiles.Remove(Target as ITile);
                };
            }
        }

        public EventTileRemove(ITile tile)
        {
            Target = tile;
            TicksRemaining = 1;
        }
    }
}
