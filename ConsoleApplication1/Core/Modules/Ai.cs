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
                var targetPoint = FindPath(target, vision);

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

            private class Node
            {
                public Point Position { get; set; }
                public int Value { get; set; }
            }

            private static Point FindPath(IUnit target, int vision)
            {
                var step = 0;
                var pointsToProcess = new List<Point>();
                pointsToProcess.Add(new Point() { X = target.X, Y = target.Y });
                var field = new int[DisplayManager.Current.FieldHeight, DisplayManager.Current.FieldWidth];
                for (int x = 0; x < DisplayManager.Current.FieldWidth; x++)
                {
                    for (int y = 0; y < DisplayManager.Current.FieldHeight; y++)
                    {
                        field[y, x] = -1;
                    }
                }

                while (step <= vision)
                {
                    var temp = new List<Point>();

                    foreach (var point in pointsToProcess)
                    {
                        if (point.Y > 0 && point.Y < DisplayManager.Current.FieldHeight && point.X > 0 && point.X < DisplayManager.Current.FieldWidth && field[point.Y, point.X] == -1)
                        {
                            if (GameManager.Current.GetTilesAt(point.X, point.Y).All(x => x.Pathable))
                            {
                                field[point.Y, point.X] = step;

                                for (int dX = point.X - 1; dX <= point.X + 1; dX++)
                                {
                                    var dY = point.Y;
                                    if (dY > 0 && dY < DisplayManager.Current.FieldHeight && dX > 0 && dX < DisplayManager.Current.FieldWidth && field[dY, dX] == -1)
                                    {
                                        temp.Add(new Point() { X = dX, Y = dY });
                                    }
                                }
                                for (int dY = point.Y - 1; dY <= point.Y + 1; dY++)
                                {
                                    var dX = point.X;
                                    if (dY > 0 && dY < DisplayManager.Current.FieldHeight && dX > 0 && dX < DisplayManager.Current.FieldWidth && field[dY, dX] == -1)
                                    {
                                        temp.Add(new Point() { X = dX, Y = dY });
                                    }
                                }
                            }

                        }
                    }

                    pointsToProcess.Clear();
                    pointsToProcess.AddRange(temp);

                    step++;
                }

                field[GameManager.Current.Player.Y, GameManager.Current.Player.X] = -2;

                var playerPosition = new Point()
                {
                    X = GameManager.Current.Player.X,
                    Y = GameManager.Current.Player.Y
                };
                var chain = new List<Node>() { new Node() { Position = playerPosition, Value = int.MaxValue } };
                var chainCountbefore = 0;

                do
                {
                    chainCountbefore = chain.Count;
                    var previous = chain.Last();

                    var nearbyPathpoints = new List<Node>();

                    for (int x = previous.Position.X - 1; x <= previous.Position.X + 1; x++)
                    {
                        if (previous.Position.Y > 0 &&
                            previous.Position.Y < DisplayManager.Current.FieldHeight &&
                            x > 0 &&
                            x < DisplayManager.Current.FieldWidth &&
                            (field[previous.Position.Y, x] >= 0))
                        {
                            nearbyPathpoints.Add(new Node() { Position = new Point() { X = x, Y = previous.Position.Y }, Value = field[previous.Position.Y, x] });
                        }
                    }

                    for (int y = previous.Position.Y - 1; y <= previous.Position.Y + 1; y++)
                    {
                        if (previous.Position.X > 0 &&
                            previous.Position.X < DisplayManager.Current.FieldWidth &&
                            y > 0 &&
                            y < DisplayManager.Current.FieldHeight &&
                            (field[y, previous.Position.X] >= 0))
                        {
                            nearbyPathpoints.Add(new Node() { Position = new Point() { X = previous.Position.X, Y = y }, Value = field[y, previous.Position.X] });
                        }
                    }

                    nearbyPathpoints = nearbyPathpoints.OrderBy(x => x.Value).ToList();

                    var mostRationalPointToGo = nearbyPathpoints.FirstOrDefault();

                    if (mostRationalPointToGo != null && previous.Value > mostRationalPointToGo.Value)
                        chain.Add(mostRationalPointToGo);

                } while (chainCountbefore != chain.Count && chain.Last().Value > 0);
                
                if (chain.Count <= 1)
                    return null;
                else
                {
                    chain.Reverse();
                    return chain.Skip(1).First().Position;
                }
                    
            }
        }
    }
}
