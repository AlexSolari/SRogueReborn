using SRogue.Core.Common;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities
{
    public abstract class Entity : IEntity, ICloneable, IMovable
    {
        public Entity()
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
                return (char)typeof(Assets).GetField(this.GetType().Name).GetValue(null);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

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

            if (GameManager.Instance.PlaceFree(targetX, targetY, false, false))
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
            if (GameManager.Instance.PlaceFree(x, y))
            {
                X = x;
                Y = y;
            }
        }
    }
}
