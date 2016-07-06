using SRogue.Core.Common.Buffs;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents
{
    public abstract class EventPeriodicDamage : TickEventBase
    {
        protected BuffBase Debuff { get; set; }

        protected float Damage { get; set; }

        public override Action Event
        {
            get
            {
                return () => {
                    (Target as IUnit).Damage(Damage);
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
                        GameManager.Current.Player.Buffs.Remove(Debuff);
                    }
                };
            }
        }

        public EventPeriodicDamage(IUnit unit, float damagePure, int ticks, BuffBase debuff)
        {
            Target = unit;
            TicksRemaining = ticks;
            Damage = damagePure;
            Debuff = debuff;
            if (Target == GameManager.Current.Player)
            {
                GameManager.Current.Player.Buffs.Add(Debuff);
            }
        }
    }
}
