using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Common.Items.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Slots
{
    public class WeaponSlot : ItemSlotBase<WeaponBase>
    {
        public WeaponSlot() : base(ItemType.Weapon)
        {
        }

        public void Ability(Direction direction)
        {
            (Item as WeaponBase).Ability(direction);
        }
    }
}
