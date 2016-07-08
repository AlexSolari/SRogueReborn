using SRogue.Core.Common;
using SRogue.Core.Entities.Concrete.Entities;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
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
            AiTicks.Add(typeof(TType), tick as Action<IUnit>);
        }

        public void RegisterAiFor(Type type, Action<IUnit> tick)
        {
            if (typeof(IUnit).IsAssignableFrom(type) && typeof(IAiControllable).IsAssignableFrom(type))
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

        public void RegisterAllFrom<TType>()
        {
            var methods = typeof(TType).GetMethods().Where(x => x.GetCustomAttributes(typeof(AiForAttribute), true).Count() > 0);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(AiForAttribute), true) as AiForAttribute[];
                foreach (var attr in attributes)
                {
                    RegisterAiFor(attr.Target, (x) => method.Invoke(null, new[] { x }) );
                }
            }
        }

        public class Container
        {
            [AiFor(typeof(Zombie))]
            [AiFor(typeof(ZombieBoss))]
            public static void ZombieAi(IUnit target)
            {
                var targetPlayer = GameManager.Current.Entities
                .Where(e => e.X < target.X + 5 && e.X > target.X - 5 && e.Y < target.Y + 5 && e.Y > target.Y - 5)
                .Where(e => e is Player)
                .FirstOrDefault();

                if (targetPlayer == null)
                {
                    target.Move((Direction)Rnd.Current.Next(4));
                    return;
                }

                var canAttack = (target.X + 1 == targetPlayer.X || target.X - 1 == targetPlayer.X || target.X == targetPlayer.X)
                    && (target.Y + 1 == targetPlayer.Y || target.Y - 1 == targetPlayer.Y || target.Y == targetPlayer.Y);

                if (canAttack)
                {
                    targetPlayer.Damage(target.Attack, Common.DamageType.Physical);
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
