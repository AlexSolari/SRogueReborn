using SRogue.Core.Common;
using SRogue.Core.Common.Buffs;
using SRogue.Core.Common.TickEvents;
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
    public class Ghost : HostileUnitBase, IDodger, IPathingIgnorer, ILootable
    {
        public float DodgeChance { get; set; }

        public bool DroppedLoot { get; set; }

        public void DropLoot()
        {
            var drop = EntityLoadManager.Current.Load<ScrollDrop>();
            drop.X = X;
            drop.Y = Y;
            GameManager.Current.OnTickEndEvents.Add(new EventItemDrop<ScrollDrop>(drop));
            DroppedLoot = true;
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

            var entities = GameState.Current.CurrentLevel.GetEntitiesAt(targetX, targetY).Where(x => x is IInteractable);

            if (entities.Count() == 0)
            {
                X = targetX;
                Y = targetY;
            }
            
            X = (X >= SizeConstants.FieldWidth) ? SizeConstants.FieldWidth - 1 : X;
            X = (X < 0) ? 0 : X;
            Y = (Y >= SizeConstants.FieldHeight) ? SizeConstants.FieldHeight - 1 : Y;
            Y = (Y < 0) ? 0 : Y;
        }

        public override void Kill(IEntity source = null)
        {
            if (!DroppedLoot)
                DropLoot();

            base.Kill(source);
        }
    }
}
