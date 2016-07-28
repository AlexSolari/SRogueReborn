using SRogue.Core.Common;
using SRogue.Core.Entities.Concrete.Entities;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class Ai
    {
        protected Dictionary<Type, Action<IUnit>> AiTicks = new Dictionary<Type, Action<IUnit>>();

        public void RegisterAiFor<TType>(Action<TType> tick)
            where TType : IAiControllable, IUnit
        {
            if (!AiTicks.Any(x => x.Key == typeof(TType)))
            {
                AiTicks.Add(typeof(TType), tick as Action<IUnit>);
            }
        }

        public void RegisterAiFor(Type type, Action<IUnit> tick)
        {
            if (typeof(IUnit).IsAssignableFrom(type) && typeof(IAiControllable).IsAssignableFrom(type) && !AiTicks.Any(x => x.Key == type))
            {
                AiTicks.Add(type, tick as Action<IUnit>);
            }
        }

        public void Execute<TType>(TType target)
            where TType : IAiControllable, IUnit
        {
            (AiTicks[typeof(TType)] as Action<TType>)(target);
        }

        public void Execute(Type type, object target)
        {
            if (target is IAiControllable && target is IUnit)
            {
                (AiTicks[type])(target as IUnit);
            }
        }

        public void InitializeDefaults()
        {
            var types = typeof(Ai).Assembly.DefinedTypes;
            var files = Directory.GetFiles("res/ai");

            foreach (var file in files)
            {
                var targets = new List<string>();
                var vision = 0;
                var radius = 0;
                var damagetype = 0;

                using (var reader = new StreamReader(file))
                {
                    var str = string.Empty;
                    while(!reader.EndOfStream)
                    {
                        str = reader.ReadLine();

                        if (str.StartsWith("#"))
                        {
                            targets.Add(str.Substring(1));
                        }
                        else if (str.StartsWith("vision:"))
                        {
                            int.TryParse(str.Substring(7), out vision);
                        }
                        else if (str.StartsWith("attackradius:"))
                        {
                            int.TryParse(str.Substring(13), out radius);
                        }
                        else if (str.StartsWith("damagetype:"))
                        {
                            int.TryParse(str.Substring(11), out damagetype);
                        }
                    }
                }

                var typeTargets = types.Where(x => targets.Contains(x.Name));
                foreach (var type in typeTargets)
                {
                    RegisterAiFor(type, (unit) => Container.Prefab(unit, vision, radius, damagetype));
                }
            }
        }

        public class Container
        {
            public static void Prefab(IUnit target, int vision, int radius, int damagetype)
            {
                var targetPlayer = GameManager.Current.Entities
                .Where(e => e.X < target.X + vision && e.X > target.X - vision && e.Y < target.Y + vision && e.Y > target.Y - vision)
                .Where(e => e is Player)
                .FirstOrDefault();

                if (targetPlayer == null)
                {
                    target.Move((Direction)Rnd.Current.Next(4));
                    return;
                }

                var canAttack = (target.X + radius == targetPlayer.X || target.X - radius == targetPlayer.X || target.X == targetPlayer.X)
                    && (target.Y + radius == targetPlayer.Y || target.Y - radius == targetPlayer.Y || target.Y == targetPlayer.Y);

                if (canAttack)
                {
                    targetPlayer.Damage(target.Attack, (Common.DamageType)damagetype);
                    UiManager.Current.Actions.Append("Taked {0} damage from {1}. ".FormatWith(target.Attack, target.GetType().Name));
                }
                else
                {
                    if (targetPlayer.X > target.X)
                        target.Move(Direction.Right);
                    else if (targetPlayer.X < target.X)
                        target.Move(Direction.Left);
                    else if (targetPlayer.Y > target.Y)
                        target.Move(Direction.Bottom);
                    else if (targetPlayer.Y < target.Y)
                        target.Move(Direction.Top);
                }
            }
        }
    }
}
