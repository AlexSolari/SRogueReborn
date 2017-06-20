using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventTileRemove : SingleTimeEvent
    {
        public EventTileRemove(ITile tile) : base(() => 
        {
            if (tile != null && GameState.Current.CurrentLevel.Tiles[tile.Y, tile.X] == tile)
                GameState.Current.CurrentLevel.Tiles[tile.Y, tile.X] = null;
        }
        )
        {
        }
    }
}
