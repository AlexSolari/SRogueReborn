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
    public abstract class Unit : IUnit, IMovable, IDamageble
    {
        protected char textureCache;

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
                if (textureCache == '\0')
                    textureCache = (char)typeof(Assets).GetField(this.GetType().Name).GetValue(null);
                return textureCache;
            }
        }

        public float Health { get; set; }

        public float HealthMax { get; set; }

        public int Armor { get; set; }

        public int MagicResist { get; set; }

        public void Move(Direction direction)
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

            if (GameManager.Current.PlaceFree(targetX, targetY, false, false))
            {
                X = targetX;
                Y = targetY;
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
            if (GameManager.Current.PlaceFree(x, y))
            {
                X = x;
                Y = y;
            }
        }

        public virtual void Damage(float pure, DamageType type)
        {
            Health -= DecreaseDamage(pure, type);
            if (Health <= 0)
            {
                Kill();
            }
        }

        public void Kill()
        {
            Health = 0;
            GameManager.Current.OnTickEndEvents.Add(new EventEntityRemove(this));
        }

        public float DecreaseDamage(float pureDamage, DamageType type)
        {
            var decreaseType = float.NaN;

            switch (type)
            {
                case DamageType.Pure:
                    decreaseType = GameplayConstants.PureDamageDecreaseComponent;
                    break;
                case DamageType.Physical:
                    decreaseType = GameplayConstants.PhysicalDamageDecreaseComponent;
                    break;
                case DamageType.Magical:
                    decreaseType = GameplayConstants.MagicalDamageDecreaseComponent;
                    break;
                default:
                    throw new ArgumentException("Unknown damage type");
            }

            return (float)Math.Pow((double)decreaseType, Armor) * pureDamage;
        }
    }
}
