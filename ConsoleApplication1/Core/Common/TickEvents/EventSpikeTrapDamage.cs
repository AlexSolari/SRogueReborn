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
        public override Action Event
        {
            get
            {
                if (Target == GameManager.Current.Player)
                {
                    UiManager.Current.Actions.Append("Taked {0} damage form bleeding. ".FormatWith(Damage));
                }
                return base.Event;
            }
        }

        public EventSpikeTrapDamage(IUnit unit)
            : base(unit, 5, DamageType.Physical, 5, new BuffInjured())
        {
        }
    }
}
