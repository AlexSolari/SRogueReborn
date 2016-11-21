using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Slots
{
    public class ItemSlotBase<TItem>
        where TItem : EquipmentBase
    {
        protected ItemType Type { get; set; }

        public TItem Item { get; private set; }

        public ItemSlotBase(ItemType type)
        {
            Type = type;
        }

        public void Equip(TItem item)
        {
            if (item == null)
                return;

            if(Type == item.Slot)
            {
                Item = item;
            }
        }

        public TItem Dequip()
        {
            var item = Item;
            Item = null;
            return item;
        }
    }
}
