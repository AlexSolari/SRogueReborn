using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Bases
{
    public class WeaponBase : EquipmentBase
    {
        protected int _Damage;

        public int Damage
        {
            get
            {
                return _Damage + (int)Material + (int)Quality;
            }
            protected set
            {
                _Damage = value;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name + " ({0} dmg)".FormatWith(Damage);   
            }
        }

        public WeaponBase(int damage)
            : base()
        {
            _Damage = damage;
            Slot = ItemType.Weapon;
        }
    }
}
