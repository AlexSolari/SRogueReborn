using SRogue.Core.Common;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities
{
    public abstract class Tile : ITile, ICloneable
    {
        public virtual bool Pathable { get; set; }

        public virtual int X { get; set; }

        public virtual int Y { get; set; }

        public virtual char Texture 
        {
            get
            {
                return (char)typeof(Assets).GetField(this.GetType().Name).GetValue(null);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }


        public virtual void OnStepOver(IEntity unit)
        {
            
        }
    }
}
