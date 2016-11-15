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
                    while (!reader.EndOfStream)
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
                var targetPoint = GetNextPointToMove(target, vision);

                if (((object)targetPoint) == null)
                {
                    target.Move((Direction)Rnd.Current.Next(4));
                    return;
                }

                var canAttack = GameManager.Current.Player.X == targetPoint.X && GameManager.Current.Player.Y == targetPoint.Y;

                if (canAttack)
                {
                    GameManager.Current.Player.Damage(target.Attack, (Common.DamageType)damagetype);
                    UiManager.Current.Actions.Append("Taked {0} damage from {1}. ".FormatWith(target.Attack, target.GetType().Name));
                }
                else
                {
                    if (targetPoint.X > target.X)
                        target.Move(Direction.Right);
                    else if (targetPoint.X < target.X)
                        target.Move(Direction.Left);
                    else if (targetPoint.Y > target.Y)
                        target.Move(Direction.Bottom);
                    else if (targetPoint.Y < target.Y)
                        target.Move(Direction.Top);
                }
            }

            #region PathFinding

            private class PathfindNode
            {
                public PathfindNode Previous { get; set; }
                public Point Position { get; set; }
                public int Value { get; set; }
            }

            private static Point GetNextPointToMove(IUnit target, int vision)
            {
                var step = 0;
                var pointsToProcess = new List<PathfindNode>();
                pointsToProcess.Add(new PathfindNode() { Position = new Point() { X = target.X, Y = target.Y }, Previous = null, Value = 0 });
                var field = new PathfindNode[DisplayManager.Current.FieldHeight, DisplayManager.Current.FieldWidth];
                for (int x = 0; x < DisplayManager.Current.FieldWidth; x++)
                {
                    for (int y = 0; y < DisplayManager.Current.FieldHeight; y++)
                    {
                        field[y, x] = new PathfindNode() { Position = new Point() { X = x, Y = y }, Previous = null, Value = -1 };
                    }
                }
                
                while (step <= vision)
                {
                    var temp = new List<PathfindNode>();

                    foreach (var point in pointsToProcess)
                    {
                        if (point.Position.Y > 0 && point.Position.Y < DisplayManager.Current.FieldHeight &&
                            point.Position.X > 0 && point.Position.X < DisplayManager.Current.FieldWidth)
                        {
                            if (GameManager.Current.GetTilesAt(point.Position.X, point.Position.Y).All(x => x.Pathable))
                            {
                                field[point.Position.Y, point.Position.X].Value = step;
                                field[point.Position.Y, point.Position.X].Previous = point?.Previous;

                                for (int dX = point.Position.X - 1; dX <= point.Position.X + 1; dX++)
                                {
                                    var dY = point.Position.Y;
                                    if (dY > 0 && dY < DisplayManager.Current.FieldHeight && dX > 0 && dX < DisplayManager.Current.FieldWidth && field[dY, dX].Value == -1)
                                    {
                                        temp.Add(new PathfindNode() { Position = new Point() { X = dX, Y = dY }, Value = step + 1, Previous = field[point.Position.Y, point.Position.X] });
                                    }
                                }
                                for (int dY = point.Position.Y - 1; dY <= point.Position.Y + 1; dY++)
                                {
                                    var dX = point.Position.X;
                                    if (dY > 0 && dY < DisplayManager.Current.FieldHeight && dX > 0 && dX < DisplayManager.Current.FieldWidth && field[dY, dX].Value == -1)
                                    {
                                        temp.Add(new PathfindNode() { Position = new Point() { X = dX, Y = dY }, Value = step + 1, Previous = field[point.Position.Y, point.Position.X] });
                                    }
                                }
                            }

                        }
                    }

                    pointsToProcess.Clear();
                    pointsToProcess.AddRange(temp);

                    step++;
                }

                var currentPoint = field[GameManager.Current.Player.Y, GameManager.Current.Player.X];
                var chain = new List<PathfindNode>();

                while (currentPoint.Previous != null)
                {
                    chain.Add(currentPoint);
                    currentPoint = currentPoint.Previous;
                }

                return chain.LastOrDefault()?.Position;
            }

            #endregion
        }
    }
}
