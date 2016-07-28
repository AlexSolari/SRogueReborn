using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Interfaces
{
    public interface ITile : IPositionable, IDisplayable, IEntity
    {
        bool Pathable { get; set; }

        void OnStep(IUnit unit);
    }
}
