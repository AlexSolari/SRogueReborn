using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Bases
{
    public class WeaponBase : ItemBase
    {
        protected int _Damage;

        public int Damage
        {
            get
            {
                return _Damage + (int)Material + (int)Quality;
            }
            set
            {
                _Damage = value;
            }
        }

        public override string Name
        {
            get
            {
                if (isEmpty)
                    return base.Name;

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
