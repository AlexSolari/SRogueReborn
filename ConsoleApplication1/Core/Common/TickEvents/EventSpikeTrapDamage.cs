using SRogue.Core.Common.Buffs;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public class EventSpikeTrapDamage : TickEventBase
    {
        private BuffInjured debuff = new BuffInjured();

        public override Action Event
        {
            get
            {
                return () => {
                    (Target as IUnit).Damage(5);
                };
            }
        }

        public override Action OnTimeout
        {
            get
            {
                return () => {
                    if (Target == GameManager.Current.Player)
                    {
                        GameManager.Current.Player.Buffs.Remove(debuff);
                    }
                };
            }
        }

        public EventSpikeTrapDamage(IUnit unit)
        {
            Target = unit;
            TicksRemaining = 5;
            if (Target == GameManager.Current.Player)
            {
                GameManager.Current.Player.Buffs.Add(debuff);
            }
        }
    }
}
