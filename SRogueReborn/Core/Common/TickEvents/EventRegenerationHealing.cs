using SRogue.Core.Common.Buffs;
using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventRegenerationHealing : TickEventBase 
    {
        public BuffBase Buff { get; set; }

        public override Action Event
        {
            get
            {
                return () => (Target as IUnit).Health = Math.Min((Target as IUnit).Health + 13, (Target as IUnit).HealthMax);
            }
        }

        public override Action OnTimeout
        {
            get
            {
                return () => {
                    if (Target == GameState.Current.Player)
                    {
                        GameState.Current.Player.Buffs.Remove(Buff);
                    }
                };
            }
        }

        public EventRegenerationHealing(IUnit unit)
            : base()
        {
            TicksRemaining = 15;
            Target = unit;
            Buff = new BuffRegeneration();
            if (Target == GameState.Current.Player)
            {
                GameState.Current.Player.Buffs.Add(Buff);
            }
        }
    }
}
