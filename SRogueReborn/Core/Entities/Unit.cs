using SRogue.Core.Common;
using SRogue.Core.Common.TickEvents;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities
{
    public abstract class Unit : IUnit
    {
        public Unit()
        {
            Id = Guid.NewGuid();
        }

        [XmlIgnore]
        public Guid Id { get; set; }

        public virtual int X { get; set; }

        public virtual int Y { get; set; }

        public virtual char Texture
        {
            get
            {
                return Assets.GetTexture(this.GetType());
            }
        }

        public float Health { get; set; }

        public float HealthMax { get; set; }

        public int Armor { get; set; }

        public int MagicResist { get; set; }

        public int Attack { get; set; }

        public bool IsAlive { get { return Health > 0; } }

        public virtual void Move(Direction direction)
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
            if (GameManager.Current.PlaceFree(targetX, targetY, false, false))
            {
                X = targetX;
                Y = targetY;
            }
            else if (entities.Count() > 0 && this is IControllable)
            {
                foreach (var entity in entities)
                {
                    (this as IControllable).Interact(entity as IInteractable);
                }
            }
        }

        public void Move(Direction direction, int count)
        {
            while (count-- > 0)
            {
                Move(direction);
            }
        }

        public void MoveInstantly(int x, int y)
        {
            if (GameManager.Current.PlaceFree(x, y, false, false))
            {
                X = x;
                Y = y;
            }
        }

        public virtual void Damage(float pure, DamageType type, IEntity source = null)
        {
            Health -= DecreaseDamage(pure, type);
            if (!IsAlive)
            {
                Kill(source);
            }
        }

        public virtual void Kill(IEntity source = null)
        {
            GameManager.Current.OnTickEndEvents.Add(new EventEntityRemove(this));
        }

        public float DecreaseDamage(float pureDamage, DamageType type)
        {
            switch (type)
            {
                case DamageType.Pure:
                    return pureDamage;
                case DamageType.Physical:
                    return (float)Math.Pow(GameplayConstants.PhysicalDamageDecreaseComponent, SummarizeArmor()) * pureDamage;
                case DamageType.Magical:
                    return (float)Math.Pow(GameplayConstants.MagicalDamageDecreaseComponent, SummarizeResist()) * pureDamage;
                default:
                    throw new ArgumentException("Unknown damage type");
            }
        }

        public virtual int SummarizeArmor()
        {
            return Armor;
        }

        public virtual int SummarizeResist()
        {
            return MagicResist;
        }

        public virtual int SummarizeAttack()
        {
            return Attack;
        }
    }
}
