using SRogue.Core.Common;
using SRogue.Core.Common.TickEvents;
using SRogue.Core.Entities;
using SRogue.Core.Entities.Concrete.Entities;
using SRogue.Core.Entities.Concrete.Tiles;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class Game
    {
        public Player Player { get; set; }
        public IList<IUnit> Entities { get; private set; }
        public IList<ITile> Tiles { get; private set; }
        public List<TickEventBase> OnTickEndEvents { get; set; }

        public Game()   
        {
            Entities = new List<IUnit>();
            Tiles = new List<ITile>();
            OnTickEndEvents = new List<TickEventBase>();
        }

        #region GameObjects
        
        public IEnumerable<ITile> GetTilesAt(int x, int y)
        {
            return Tiles.Where(t => t.X == x && t.Y == y);
        }

        public IEnumerable<IUnit> GetEntitiesAt(int x, int y)
        {
            return Entities.Where(t => t.X == x && t.Y == y);
        }

        public void Add(IUnit entity)
        {
            Entities.Add(entity);
        }

        public void Add(ITile tile)
        {
            var tiles = GetTilesAt(tile.X, tile.Y);

            foreach (var t in tiles)
            {
                OnTickEndEvents.Add(new EventTileRemove(t));
            }

            Tiles.Add(tile);
        }

        #endregion

        #region GameStatus

        public void ProcessInput(char input)
        {
            input = char.ToLower(input);
            switch (input)
            {
                case 'w':
                    Player.Move(Direction.Top);
                    break;
                case 's':
                    Player.Move(Direction.Bottom);
                    break;
                case 'a':
                    Player.Move(Direction.Left);
                    break;
                case 'd':
                    Player.Move(Direction.Right);
                    break;
                default:
                    break;
            }
        }

        public void GameTick()
        {
            foreach (var entity in Entities)
            {
                if (entity is IAiControllable)
                {
                    (entity as IAiControllable).AiTick();
                }

                var tilesUnderEntity = GetTilesAt(entity.X, entity.Y);

                foreach (var tile in tilesUnderEntity)
                {
                    tile.OnStep(entity);
                }

                if (entity.Health < entity.HealthMax)
                {
                    entity.Health = Math.Min(entity.Health + 1, entity.HealthMax);
                }
            }

            var events = OnTickEndEvents.ToList();
            foreach (var evnt in events)
            {
                evnt.Event();
                evnt.TicksRemaining--;
                if (evnt.TicksRemaining == 0 && evnt.OnTimeout != null)
                {
                    evnt.OnTimeout();
                }
            }

            OnTickEndEvents.RemoveAll(x => x.TicksRemaining <= 0);
        }

        public bool PlaceFree(int x, int y, bool ignoreEntities = false, bool ignorePathable = true)
        {
            var isThereEntities = Entities.Any(e => e.X == x && e.Y == y);
            var tileAtPoint = Tiles.Where(t => t.X == x && t.Y == y);
            var isThereTiles = (ignorePathable) ? tileAtPoint.Any() : tileAtPoint.Any(t => !t.Pathable);

            return (ignoreEntities || !isThereEntities) && !isThereTiles;
        }

        public ITile GetRandomTile(bool pathable = false)
        {
            var tiles = Tiles.Where(x => (pathable) ? x.Pathable : !x.Pathable).ToList();
            var tile = tiles[Rnd.Current.Next(tiles.Count())];
            return tile;
        }

        #endregion

        #region WorldGenerator
       
        public void GenerateWorld()
        {
            GameState.Current.Depth++;

            Tiles.Clear();
            Entities.Clear();

            var roomsCount = Rnd.Current.Next(6, 14);
            var centers = new List<Point>();
            for (int i = 0; i < roomsCount; i++)
            {
                GenerateRoom(centers);
            }

            GenerateCorridors(centers);

            Fill();

            GenerateTraps();

            AddPlayer(centers);

            GenerateEnemies();

            GenerateExit(centers);
#if !DEBUG
            DisplayManager.Current.ResetOverlay();
#endif
        }

        private void AddPlayer(IList<Point> centers)
        {
            Player = EntityLoadManager.Current.Load<Player>();
            Player.X = centers.First().X;
            Player.Y = centers.First().Y;
            Add(Player);
        }

        protected void GenerateExit(IList<Point> centers)
        {
            var exit = EntityLoadManager.Current.Load<Exit>();
            exit.X = centers.Last().X;
            exit.Y = centers.Last().Y;
            Add(exit);
        }

        protected void GenerateEnemies()
        {
            for (int index = 0; index < GameState.Current.Depth + 2; index++)
            {
                var zombie = EntityLoadManager.Current.Load<Zombie>();
                var tile = GetRandomTile(true);
                zombie.X = tile.X;
                zombie.Y = tile.Y;
                Add(zombie);
            }
        }

        protected void Fill()
        {
            for (int x = 0; x < DisplayManager.Current.FieldWidth; x++)
            {
                for (int y = 0; y < DisplayManager.Current.FieldHeight; y++)
                {
                    if (PlaceFree(x, y, true))
                    {
                        var tile = EntityLoadManager.Current.Load<Wall>();

                        tile.X = x;
                        tile.Y = y;

                        Add(tile);
                    }
                }
            }
        }

        protected void GenerateCorridors(IList<Point> centers)
        {
            foreach (var current in centers)
            {
                var target = centers.Where(p => p != current).OrderBy(p => Rnd.Current.Next()).FirstOrDefault();

                var x = current.X;
                var y = current.Y;
                while (x != target.X)
                {
                    var tile = EntityLoadManager.Current.Load<Floor>();

                    tile.X = x;
                    tile.Y = y;

                    Add(tile);

                    x = x.GoesTo(target.X);
                }
                while (y != target.Y)
                {
                    var tile = EntityLoadManager.Current.Load<Floor>();

                    tile.X = x;
                    tile.Y = y;

                    Add(tile);

                    y = y.GoesTo(target.Y);
                }
            }
        }

        protected void GenerateRoom(IList<Point> centers)
        {
            var roomSizeX = Rnd.Current.Next(3, 5);
            var roomSizeY = Rnd.Current.Next(2, 3);
            var roomX = Rnd.Current.Next(roomSizeX + 1, DisplayManager.Current.FieldWidth - 1 - roomSizeX);
            var roomY = Rnd.Current.Next(roomSizeY + 1, DisplayManager.Current.FieldHeight - 1 - roomSizeY);

            centers.Add(new Point() { X = roomX, Y = roomY });

            for (int x = roomX - roomSizeX; x <= roomX + roomSizeX; x++)
            {
                for (int y = roomY - roomSizeY; y <= roomY + roomSizeY; y++)
                {
                    var tile = EntityLoadManager.Current.Load<Floor>();
                    
                    tile.X = x;
                    tile.Y = y;

                    Add(tile);
                }
            }
        }

        protected void GenerateTraps()
        {
            for (int i = 0; i < Rnd.Current.Next(5,15); i++)
            {
                var oldTile = GetRandomTile(true);
                var newTile = EntityLoadManager.Current.Load<SpikeTrap>();

                newTile.X = oldTile.X;
                newTile.Y = oldTile.Y;

                Add(newTile);
            }
        }
#endregion

    }
}
