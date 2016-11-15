using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Sword : WeaponBase
    {
        public Sword(int damage = 1)
            : base(damage)
        {

        }
    }
}
