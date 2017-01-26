using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Wand : WeaponBase
    {
        public virtual int MagicDamage
        {
            get
            {
                return _Damage + (int)Material + (int)Quality;
            }
            protected set
            {
                _Damage = value;
            }
        }

        public override int Damage
        {
            get
            {
                return 1;
            }

            protected set
            {
                base.Damage = value;
            }
        }

        public override string Name
        {
            get
            {
                return "{0} {1} {2}".FormatWith(Quality, Material, this.GetType().Name) + " ({0} melee | {1} ranged)".FormatWith(Damage, MagicDamage);
            }
        }

        public override void Ability(Direction direction)
        {
            var currentPosition = new Point() { X = GameState.Current.Player.X, Y = GameState.Current.Player.Y };
            IEnumerable<ITile> currentTiles;

            do
            {
                switch (direction)
                {
                    case Direction.Top:
                        currentPosition.Y--;
                        break;
                    case Direction.Bottom:
                        currentPosition.Y++;
                        break;
                    case Direction.Left:
                        currentPosition.X--;
                        break;
                    case Direction.Right:
                        currentPosition.X++;
                        break;
                    default:
                        break;
                }

                currentTiles = GameManager.Current.GetTilesAt(currentPosition.X, currentPosition.Y);

                if (currentTiles.Any(x => !x.Pathable))
                    break;

                DisplayManager.Current.BlastedPoints.Add(new Point() { X = currentPosition.X, Y = currentPosition.Y });

                var entities = GameManager.Current.GetEntitiesAt(currentPosition.X, currentPosition.Y);

                foreach (var entity in entities)
                {
                    entity.Damage(MagicDamage, DamageType.Magical, GameState.Current.Player);
                }
            }
            while (currentTiles.Any());

            GameManager.Current.BlastFired = true;

            base.Ability(direction);
        }

        public Wand(int damage = 1)
            : base(damage)
        {

        }
    }
}
