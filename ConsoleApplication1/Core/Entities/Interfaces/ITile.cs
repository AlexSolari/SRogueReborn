using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Interfaces
{
    public interface ITile : IPositionable, IDisplayable
    {
        bool Pathable { get; set; }

        void OnStepOver(IEntity unit);
    }
}
