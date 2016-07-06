using SRogue.Core.Common.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class Player : Unit
    {
        [XmlIgnore]
        public List<BuffBase> Buffs { get; set; }

        public Player()
        {
            Buffs = new List<BuffBase>();
        }
    }
}
