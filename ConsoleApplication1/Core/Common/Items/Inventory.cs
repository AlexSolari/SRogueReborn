using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items
{
    public class Inventory
    {
        public ItemSlot Head { get; set; }
        public ItemSlot Chest { get; set; }
        public ItemSlot Legs { get; set; }
        public ItemSlot Foot { get; set; }
        public ItemSlot Weapon { get; set; }

        public IList<ItemBase> Backpack { get; set; }

        public Inventory()
        {
            Backpack = new List<ItemBase>();

            Head = new ItemSlot(ItemType.Head);
            Chest = new ItemSlot(ItemType.Chest);
            Legs = new ItemSlot(ItemType.Legs);
            Foot = new ItemSlot(ItemType.Foot);
            Weapon = new ItemSlot(ItemType.Weapon);
        }
    }
}
