using SRogue.Core.Common;
using SRogue.Core.Common.Buffs;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class Zombie : Unit, IAiControllable, IHostile
    {
        public void AiTick()
        {
            var generator = new Random();
            var targetPlayer = GameManager.Current.Entities
                .Where(e => e.X < X + 5 && e.X > X - 5 && e.Y < Y + 5 && e.Y > Y - 5)
                .Where(e => e is Player)
                .FirstOrDefault();

            if (targetPlayer == null)
            {
                Move((Direction)generator.Next(4));
                return;
            }

            var canAttack = (X + 1 == targetPlayer.X || X - 1 == targetPlayer.X || X == targetPlayer.X)
                && (Y + 1 == targetPlayer.Y || Y - 1 == targetPlayer.Y || Y == targetPlayer.Y);

            if (canAttack)
            {
                targetPlayer.Damage(Attack, Common.DamageType.Physical);
                UiManager.Current.Actions.Append("Taked {0} damage form Zombie. ".FormatWith(Attack));
            }
            else
            {
                if (targetPlayer.X > X)
                    Move(Direction.Right);
                else if (targetPlayer.X < X)
                    Move(Direction.Left);
                else if (targetPlayer.Y > Y)
                    Move(Direction.Bottom);
                else if (targetPlayer.Y < Y)
                    Move(Direction.Top);
            }
        }
    }
}
