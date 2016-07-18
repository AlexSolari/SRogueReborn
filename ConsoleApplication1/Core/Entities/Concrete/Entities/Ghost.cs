using SRogue.Core.Common;
using SRogue.Core.Common.Buffs;
using SRogue.Core.Entities.Concrete.Entities.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class Ghost : HostileUnitBase
    {
        public override void Damage(float pure, DamageType type)
        {
            if (type == DamageType.Physical && Rnd.Current.NextDouble() < 0.5)
            {
                UiManager.Current.Actions.Append("Ghost dodged. ");
                return;
            }

            base.Damage(pure, type);
        }

        public override void Move(Direction direction)
        {
            int targetX = X;
            int targetY = Y;

            switch (direction)
            {
                case Direction.Top:
                    targetY--;
                    break;
                case Direction.Bottom:
                    targetY++;
                    break;
                case Direction.Left:
                    targetX--;
                    break;
                case Direction.Right:
                    targetX++;
                    break;
                default:
                    break;
            }

            var entities = GameManager.Current.GetEntitiesAt(targetX, targetY).Where(x => x is IInteractable);

            if (entities.Count() == 0)
            {
                X = targetX;
                Y = targetY;
            }

            if (entities.Count() > 0 && this is IControllable)
            {
                foreach (var entity in entities)
                {
                    (this as IControllable).Interact(entity as IInteractable);
                }
            }

            X = (X >= DisplayManager.Current.FieldWidth) ? DisplayManager.Current.FieldWidth - 1 : X;
            X = (X < 0) ? 0 : X;
            Y = (Y >= DisplayManager.Current.FieldHeight) ? DisplayManager.Current.FieldHeight - 1 : Y;
            Y = (Y < 0) ? 0 : Y;
        }
    }
}
