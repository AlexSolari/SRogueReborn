using SRogue.Core.Common;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities
{
    public abstract class Tile : ITile
    {
        public Tile()
        {
            Id = Guid.NewGuid();
        }
        
        [XmlIgnore]
        public Guid Id { get; set; }

        public virtual bool Pathable { get; set; }

        public virtual int X { get; set; }

        public virtual int Y { get; set; }

        public virtual char Texture 
        {
            get
            {
                return Assets.GetTexture(this.GetType());
            }
        }

        public abstract void OnStep(IUnit unit);
    }
}
