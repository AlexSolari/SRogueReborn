using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Bases
{
    public class WeaponBase : ItemBase
    {
        public int Damage { get; set; }

        public WeaponBase(int damage)
            : base()
        {
            Damage = damage + ((int)Quality / 2);
            Slot = ItemType.Weapon;
        }
    }
}
