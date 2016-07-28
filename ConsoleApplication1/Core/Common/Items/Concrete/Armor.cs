using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Armor : ArmorBase
    {
        public Armor(int armor = 2, int resist = 1)
            : base(armor, resist, ItemType.Chest)
        {

        }
    }
}
