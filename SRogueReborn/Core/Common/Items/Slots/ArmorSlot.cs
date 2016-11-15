using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Common.Items.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Slots
{
    public class ArmorSlot<TArmorType> : ItemSlotBase<TArmorType>
        where TArmorType : ArmorBase
    {
        public ArmorSlot() : base(ResolveArmorType())
        {
        }

        private static ItemType ResolveArmorType()
        {
            var type = typeof(TArmorType);
            ItemType result;

            if (type == typeof(Helmet))
            {
                result = ItemType.Head;
            }
            else if (type == typeof(Boots))
            {
                result = ItemType.Foot;
            }
            else if (type == typeof(Leggins))
            {
                result = ItemType.Legs;
            }
            else
            {
                result = ItemType.Chest;
            }

            return result;
        }
    }
}
