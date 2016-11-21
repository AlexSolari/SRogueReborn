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
    public class EventStoneSkin : TickEventBase 
    {
        public int ArmorAdded { get; set; } = 50;

        public BuffBase Buff { get; set; }

        public override Action Event
        {
            get
            {
                return () => { };
            }
        }

        public override Action OnTimeout
        {
            get
            {
                return () => {
                    (Target as IUnit).Armor -= ArmorAdded;
                    if (Target == GameState.Current.Player)
                    {
                        GameState.Current.Player.Buffs.Remove(Buff);
                    }
                };
            }
        }

        public EventStoneSkin(IUnit unit)
            : base()
        {
            unit.Armor += ArmorAdded;
            TicksRemaining = 20;
            Target = unit;
            Buff = new BuffStoneSkin();
            if (Target == GameState.Current.Player)
            {
                GameState.Current.Player.Buffs.Add(Buff);
            }
        }
    }
}
