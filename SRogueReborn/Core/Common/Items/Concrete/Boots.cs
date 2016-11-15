using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Boots : ArmorBase
    {
        public Boots(int armor = 1, int resist = 0)
            : base(armor, resist, ItemType.Foot)
        {

        }
    }
}
