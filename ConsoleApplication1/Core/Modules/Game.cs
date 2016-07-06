using SRogue.Core.Common;
using SRogue.Core.Entities;
using SRogue.Core.Entities.Concrete.Entities;
using SRogue.Core.Entities.Concrete.Tiles;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class Game
    {
        public Player Player { get; set; }
        public IList<IEntity> Entities { get; private set; }
        public IList<ITile> Tiles { get; private set; }
        public List<TickEvents> OnTickEndEvents { get; set; }

        public Game()   
        {
            Entities = new List<IEntity>();
            Tiles = new List<ITile>();
            OnTickEndEvents = new List<TickEvents>();
        }

        #region GameObjects
        
        public IList<ITile> GetTilesAt(int x, int y)
        {
            return Tiles.Where(t => t.X == x && t.Y == y).ToList();
        }

        public IList<IEntity> GetEntitiesAt(int x, int y)
        {
            return Entities.Where(t => t.X == x && t.Y == y).ToList();
        }

        public void Add(IEntity entity)
        {
            Entities.Add(entity);
        }

        public void Add(ITile tile)
        {
            var tiles = GetTilesAt(tile.X, tile.Y);

            foreach (var t in tiles)
            {
                Tiles.Remove(t);
            }

            Tiles.Add(tile);
        }

        #endregion

        #region GameState

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
                var tilesUnderEntity = GetTilesAt(entity.X, entity.Y);

                foreach (var tile in tilesUnderEntity)
                {
                    tile.OnStepOver(entity);
                }
            }

            foreach (var evnt in OnTickEndEvents)
            {
                evnt.Event();
                evnt.TicksRemaining--;
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
        #endregion

        #region WorldGenerator
       
        public void GenerateWorld()
        {
            Tiles.Clear();
            Entities.Clear();

            var centers = new List<Point>();
            for (int i = 0; i < 5; i++)
            {
                var seed = DateTime.Now.Ticks / (Tiles.Count + 1);
                GenerateRoom(seed, centers);
            }

            GenerateCorridors(centers);

            var exit = EntityLoadManager.Instance.Load<Exit>();
            exit.X = centers.Last().X;
            exit.Y = centers.Last().Y;
            Add(exit);

            Player = EntityLoadManager.Instance.Load<Player>();
            Player.X = centers.First().X;
            Player.Y = centers.First().Y;
            Add(Player);

            Fill();
        }

        protected void Fill()
        {
            for (int x = 0; x < DisplayManager.Instance.Width; x++)
            {
                for (int y = 0; y < DisplayManager.Instance.Height; y++)
                {
                    if (PlaceFree(x, y, true))
                    {
                        var tile = EntityLoadManager.Instance.Load<Wall>();

                        tile.X = x;
                        tile.Y = y;

                        Add(tile);
                    }
                }
            }
        }

        protected void GenerateCorridors(IList<Point> centers)
        {
            var generator = new Random();
            foreach (var current in centers)
            {
                var target = centers.Where(p => p != current).OrderBy(p => generator.Next()).FirstOrDefault();

                var x = current.X;
                var y = current.Y;
                while (x != target.X)
                {
                    var tile = EntityLoadManager.Instance.Load<Floor>();

                    tile.X = x;
                    tile.Y = y;

                    Add(tile);

                    x = x.GoesTo(target.X);
                }
                while (y != target.Y)
                {
                    var tile = EntityLoadManager.Instance.Load<Floor>();

                    tile.X = x;
                    tile.Y = y;

                    Add(tile);

                    y = y.GoesTo(target.Y);
                }
            }
        }

        protected void GenerateRoom(long seed, IList<Point> centers)
        {
            var generator = new Random((int)seed);
            var roomSizeX = generator.Next(3, 6);
            var roomSizeY = generator.Next(2, 4);
            var roomX = generator.Next(roomSizeX + 1, DisplayManager.Instance.Width - 1 - roomSizeX);
            var roomY = generator.Next(roomSizeY + 1, DisplayManager.Instance.Height - 1 - roomSizeY);

            centers.Add(new Point() { X = roomX, Y = roomY });

            for (int x = roomX - roomSizeX; x <= roomX + roomSizeX; x++)
            {
                for (int y = roomY - roomSizeY; y <= roomY + roomSizeY; y++)
                {
                    var tile = EntityLoadManager.Instance.Load<Floor>();
                    
                    tile.X = x;
                    tile.Y = y;

                    Add(tile);
                }
            }
        }
#endregion

    }
}
