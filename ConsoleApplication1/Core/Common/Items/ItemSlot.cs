using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items
{
    public class ItemSlot
    {
        protected ItemType Type { get; set; }

        public ItemBase Item { get; private set; }

        public ItemSlot(ItemType type)
        {
            Type = type;
        }

        public void Equip(ItemBase item)
        {
            if(Type == item.Slot)
            {
                Item = item;
            }
        }

        public ItemBase Dequip()
        {
            var item = Item;
            Item = null;
            return item;
        }
    }
}
