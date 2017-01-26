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
                    RegisterAiFor(type, Container.CreatePrefab(vision, radius, damagetype));
                }
            }
        }

        public class Container
        {
            public static Action<IUnit> CreatePrefab(int vision, int radius, int damagetype)
            {
                return (unit) => Prefab(unit, vision, radius, damagetype);
            }

            private static void Prefab(IUnit target, int vision, int radius, int damagetype)
            {
                Point targetPoint = null;

                if (GetDistanceToPlayer(target) <= vision)
                {
                    targetPoint = GetNextPoint(target, vision);
                }

                if (((object)targetPoint) == null)
                {
                    target.Move((Direction)Rnd.Current.Next(4));
                    return;
                }

                var canAttack = GameState.Current.Player.X == targetPoint.X && GameState.Current.Player.Y == targetPoint.Y;

                if (canAttack)
                {
                    GameState.Current.Player.Damage(target.Attack, (Common.DamageType)damagetype, target);
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

            private static int GetDistanceToPlayer(IUnit from)
            {
                int dX = Math.Abs(from.X - GameState.Current.Player.X);
                int dY = Math.Abs(from.Y - GameState.Current.Player.Y);

                int min = Math.Min(dX, dY);
                int max = Math.Max(dX, dY);

                int diagonalSteps = min;
                int straightSteps = max - min;

                return diagonalSteps + straightSteps;
            }

            private class PathfindNode
            {
                public PathfindNode Previous { get; set; }
                public Point Position { get; set; }
                public int Value { get; set; }

                public bool Equals(PathfindNode obj)
                {
                    return this.GetHashCode() == obj.GetHashCode();
                }

                public override int GetHashCode()
                {
                    return Position.X * 31 + Position.Y;
                }
            }
            
            private static Point GetNextPoint(IUnit from, int vision)
            {
                var currentPoint = PathMap[from.Y, from.X];
                var chain = new List<PathfindNode>();

                while (currentPoint.Previous != null)
                {
                    currentPoint = currentPoint.Previous;
                    chain.Add(currentPoint);
                    
                }

                return chain.FirstOrDefault()?.Position;
            }

            private static PathfindNode[,] PathMap;

            public static void ResetPathMap()
            {
                PathMap = null;
            }

            public static void RecalculatePlayerPathmap()
            {
                CreateOrErasePathMap();

                var step = 0;
                var pointsToProcess = new List<PathfindNode>();
                pointsToProcess.Add(new PathfindNode() { Position = new Point() { X = GameState.Current.Player.X, Y = GameState.Current.Player.Y }, Previous = null, Value = 0 });

                while (true)
                {
                    if (!pointsToProcess.Any())
                        break;

                    var temp = new List<PathfindNode>();

                    foreach (var point in pointsToProcess)
                    {
                        PathMap[point.Position.Y, point.Position.X].Value = step;
                        PathMap[point.Position.Y, point.Position.X].Previous = point.Previous;

                        for (int dX = point.Position.X - 1; dX <= point.Position.X + 1; dX++)
                        {
                            var dY = point.Position.Y;
                            if (dY > 0 && dY < DisplayManager.Current.FieldHeight && dX > 0 && dX < DisplayManager.Current.FieldWidth && PathMap[dY, dX].Value == -1 && !temp.Any(x => x.Equals(PathMap[dY, dX])))
                            {
                                temp.Add(new PathfindNode() { Position = new Point() { X = dX, Y = dY }, Value = step + 1, Previous = PathMap[point.Position.Y, point.Position.X] });
                            }

                        }
                        for (int dY = point.Position.Y - 1; dY <= point.Position.Y + 1; dY++)
                        {
                            var dX = point.Position.X;
                            if (dY > 0 && dY < DisplayManager.Current.FieldHeight && dX > 0 && dX < DisplayManager.Current.FieldWidth && PathMap[dY, dX].Value == -1 && !temp.Any(x => x.Equals(PathMap[dY, dX])))
                            {
                                temp.Add(new PathfindNode() { Position = new Point() { X = dX, Y = dY }, Value = step + 1, Previous = PathMap[point.Position.Y, point.Position.X] });
                            }
                        }
                    }

                    pointsToProcess.Clear();
                    pointsToProcess.AddRange(temp);

                    step++;
                }
            }

            private static void CreateOrErasePathMap()
            {
                var noMapCreated = PathMap == null;

                if (noMapCreated)
                {
                    PathMap = new PathfindNode[DisplayManager.Current.FieldHeight, DisplayManager.Current.FieldWidth];
                }

                for (int x = 0; x < DisplayManager.Current.FieldWidth; x++)
                {
                    for (int y = 0; y < DisplayManager.Current.FieldHeight; y++)
                    {
                        if (noMapCreated)
                        {
                            PathMap[y, x] = new PathfindNode() { Position = new Point() { X = x, Y = y }, Previous = null, Value = -1 };
                        }
                        else
                        {
                            PathMap[y, x].Value = -1;
                            PathMap[y, x].Previous = null;
                        }

                        if (!GameManager.Current.GetTilesAt(x, y).Any(p => p.Pathable))
                            PathMap[y, x].Value = -2;
                    }
                }
            }

            #endregion
        }
    }
}
