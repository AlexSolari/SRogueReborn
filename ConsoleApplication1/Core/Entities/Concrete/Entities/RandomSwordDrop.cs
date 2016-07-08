using SRogue.Core.Common.Items.Concrete;
using SRogue.Core.Entities.Concrete.Entities.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class RandomSwordDrop : DropUnitBase
    {
        public RandomSwordDrop()
            :base()
        {
            Droplist.Add(new Sword());
            Droplist.Add(new Sword());
            Droplist.Add(new Sword());
            Droplist.Add(new Sword());
            Droplist.Add(new Sword());
        }
    }
}
