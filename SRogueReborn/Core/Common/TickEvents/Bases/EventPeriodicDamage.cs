using SRogue.Core.Common.Buffs;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.TickEvents.Bases
{
    public abstract class EventPeriodicDamage : TickEventBase
    {
        protected BuffBase Debuff { get; set; }

        protected float Damage { get; set; }

        protected DamageType DamageType { get; set; }

        public override Action Event
        {
            get
            {
                return () => {
                    (Target as IUnit).Damage(Damage, DamageType);
                };
            }
        }

        public override Action OnTimeout
        {
            get
            {
                return () => {
                    if (Target == GameState.Current.Player)
                    {
                        GameState.Current.Player.Buffs.Remove(Debuff);
                    }
                };
            }
        }

        public EventPeriodicDamage(IUnit unit, float damagePure, DamageType type, int ticks, BuffBase debuff)
        {
            Target = unit;
            TicksRemaining = ticks;
            Damage = damagePure;
            Debuff = debuff;
            DamageType = type;
            if (Target == GameState.Current.Player)
            {
                GameState.Current.Player.Buffs.Add(Debuff);
            }
        }
    }
}
