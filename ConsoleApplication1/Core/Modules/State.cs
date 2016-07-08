using SRogue.Core.Common.Items;
using SRogue.Core.Common.Items.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class State
    {
        public int Depth { get; set; }

        public Inventory Inventory { get; set; }

        public int Gold { get; set; }

        public State()
        {
            Inventory = new Inventory();
            Inventory.Weapon.Equip(new Sword() { Quality = ItemQuality.Common, Material = ItemMaterial.Iron });
        }
    }
}
