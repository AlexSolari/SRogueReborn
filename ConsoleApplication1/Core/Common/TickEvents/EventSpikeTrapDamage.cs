using SRogue.Core.Common.Buffs;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventSpikeTrapDamage : EventPeriodicDamage 
    {
        public EventSpikeTrapDamage(IUnit unit)
            : base(unit, 5, DamageType.Physical, 5, new BuffInjured())
        {
        }
    }
}
