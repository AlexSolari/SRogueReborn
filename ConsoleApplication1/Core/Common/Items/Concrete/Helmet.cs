using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Helmet : ArmorBase
    {
        public Helmet(int armor = 1, int resist = 0)
            : base(armor, resist, ItemType.Head)
        {

        }
    }
}
