using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Leggins : ArmorBase
    {
        public Leggins(int armor = 1, int resist = 1)
            : base(armor, resist, ItemType.Legs)
        {

        }
    }
}
