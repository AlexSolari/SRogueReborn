using SRogue.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Interfaces
{
    public interface IDamageble : IKillable
    {
        void Damage(float pure, DamageType type, IEntity source = null);
    }
}
