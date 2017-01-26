using SRogue.Core.Common;
using SRogue.Core.Common.Buffs;
using SRogue.Core.Common.TickEvents;
using SRogue.Core.Entities.Concrete.Entities.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class ZombieBoss : HostileUnitBase, ILootable
    {
        public bool DroppedLoot { get; set; }

        public void DropLoot()
        {
            var drop = EntityLoadManager.Current.Load<RandomItemDrop>();
            drop.X = X;
            drop.Y = Y;
            GameManager.Current.OnTickEndEvents.Add(new EventItemDrop<RandomItemDrop>(drop));
            DroppedLoot = true;
        }

        public override void Kill(IEntity source = null)
        {
            if (!DroppedLoot)
                DropLoot();

            base.Kill(source);
        }
    }
}
