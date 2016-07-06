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
    public abstract class Tile : ITile, ICloneable
    {
        protected char textureCache;

        [XmlIgnore]
        public Guid Id { get; set; }

        public virtual bool Pathable { get; set; }

        public virtual int X { get; set; }

        public virtual int Y { get; set; }

        public virtual char Texture 
        {
            get
            {
                if (textureCache == '\0')
                    textureCache = (char)typeof(Assets).GetField(this.GetType().Name).GetValue(null);
                return textureCache;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }


        public virtual void OnStep(IUnit unit)
        {
            
        }
    }
}
