using SRogue.Core.Common.Items.Concrete;
using SRogue.Core.Entities.Concrete.Entities.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class GoldDrop : DropUnitBase
    {
        public GoldDrop()
            :base()
        {
            Droplist.Add(new Gold());
        }
    }
}
