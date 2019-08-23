using SRogue.Core.Common.TickEvents;
using SRogue.Core.Entities.Concrete.Entities;
using SRogue.Core.Entities.Concrete.Tiles;
using SRogue.Core.Entities.Interfaces;
using SRogue.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.World
{
    public class GameLevel
    {
        public IList<IUnit> Entities { get; set; } = new List<IUnit>();
        public ITile[,] Tiles { get; set; } = new ITile[SizeConstants.FieldHeight, SizeConstants.FieldWidth];

        public ITile GetTileAt(int x, int y)
        {
            return Tiles[y, x];
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
            var oldtile = GetTileAt(tile.X, tile.Y);

            GameManager.Current.OnTickEndEvents.Add(new EventTileRemove(oldtile));

            Tiles[tile.Y, tile.X] = tile;
        }

        public bool PlaceFree(int x, int y, bool ignoreEntities = false, bool ignorePathable = true)
        {
            var isThereEntities = Entities.Any(e => e.X == x && e.Y == y);
            var tileAtPoint = Tiles[y, x];
            var isThereTiles = (ignorePathable) ? tileAtPoint != null : !tileAtPoint.Pathable;

            return (ignoreEntities || !isThereEntities) && !isThereTiles;
        }

        public ITile GetRandomTile(bool pathable = false, bool withoutEntities = true)
        {
            var rnd = Rnd.Current;
            var allTiles = Tiles;
            var pathableTiles = new List<ITile>();
            var withoutEntitiesTiles = new List<ITile>();
            var withoutEntitiesPathableTiles = new List<ITile>();

            foreach (var tile in allTiles)
            {
                var entities = GetEntitiesAt(tile.X, tile.Y);

                if (!entities.Any())
                    withoutEntitiesTiles.Add(tile);

                if (tile.Pathable)
                    pathableTiles.Add(tile);

                if (!entities.Any() && tile.Pathable)
                    withoutEntitiesPathableTiles.Add(tile);
            }

            ITile result;

            if (pathable && withoutEntities)
                result = withoutEntitiesPathableTiles[rnd.Next(withoutEntitiesPathableTiles.Count)];
            else if (pathable)
                result = pathableTiles[rnd.Next(pathableTiles.Count)];
            else if (withoutEntities)
                result = withoutEntitiesTiles[rnd.Next(withoutEntitiesTiles.Count)];
            else
                result = allTiles[rnd.Next(25), rnd.Next(59)];

            return result;
        }

        public void AddGoldPickups()
        {
            var count = Rnd.Current.Next(3, 8);
            for (int i = 0; i < count; i++)
            {
                var drop = EntityLoadManager.Current.Load<GoldDrop>();
                var tile = GetRandomTile(true);
                drop.X = tile.X;
                drop.Y = tile.Y;
                Add(drop);
            }
        }

        public void AddPlayer(IList<Point> centers)
        {
            var player = GameState.Current.Player;
            player.Health = player.HealthMax;
            player.X = centers.First().X;
            player.Y = centers.First().Y;
            Add(player);
        }

        public void GenerateExit(IList<Point> centers)
        {
            var exit = EntityLoadManager.Current.Load<Exit>();
            exit.X = centers.Last().X;
            exit.Y = centers.Last().Y;
            Add(exit);
        }

        public void GenerateCity(IList<Point> centers)
        {
            var shop = EntityLoadManager.Current.Load<ItemShop>();
            shop.X = centers.Skip(1).First().X;
            shop.Y = centers.Skip(1).First().Y;
            Add(shop);
        }

        public void GenerateEnemies(bool isBoss)
        {
            if (isBoss)
            {
                for (int index = 0; index < GameState.Current.Depth / 4; index++)
                {
                    var zombie = EntityLoadManager.Current.Load<ZombieBoss>();
                    var tile = GetRandomTile(true);
                    zombie.X = tile.X;
                    zombie.Y = tile.Y;
                    Add(zombie);
                }
            }
            else
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

            if (GameState.Current.Depth > 2)
            {
                if (Rnd.Current.NextDouble() > 0.5)
                {
                    var count = Rnd.Current.Next(1, GameState.Current.Depth / 2);
                    for (int i = 0; i < count; i++)
                    {
                        var ghost = EntityLoadManager.Current.Load<Ghost>();
                        var tile = GetRandomTile(true);
                        ghost.X = tile.X;
                        ghost.Y = tile.Y;
                        Add(ghost);
                    }

                }
            }
        }

        public void Fill()
        {
            for (int x = 0; x < SizeConstants.FieldWidth; x++)
            {
                for (int y = 0; y < SizeConstants.FieldHeight; y++)
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

        public void GenerateCorridors(IList<Point> centers)
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

            //Additional loop to make corridor from first room to last
            var targetAdditional = centers.LastOrDefault();

            var xAdditional = centers[0].X;
            var yAdditional = centers[0].Y;
            while (xAdditional != targetAdditional.X)
            {
                var tile = EntityLoadManager.Current.Load<Floor>();

                tile.X = xAdditional;
                tile.Y = yAdditional;

                Add(tile);

                xAdditional = xAdditional.GoesTo(targetAdditional.X);
            }
            while (yAdditional != targetAdditional.Y)
            {
                var tile = EntityLoadManager.Current.Load<Floor>();

                tile.X = xAdditional;
                tile.Y = yAdditional;

                Add(tile);

                yAdditional = yAdditional.GoesTo(targetAdditional.Y);
            }
        }

        public void GenerateRoom(IList<Point> centers)
        {
            var roomSizeX = Rnd.Current.Next(3, 5);
            var roomSizeY = Rnd.Current.Next(2, 3);
            var roomX = Rnd.Current.Next(roomSizeX + 1, SizeConstants.FieldWidth - 1 - roomSizeX);
            var roomY = Rnd.Current.Next(roomSizeY + 1, SizeConstants.FieldHeight - 1 - roomSizeY);

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

        public void GenerateTraps()
        {
            for (int i = 0; i < Rnd.Current.Next(5, 15); i++)
            {
                var oldTile = GetRandomTile(true);
                var newTile = EntityLoadManager.Current.Load<SpikeTrap>();

                newTile.X = oldTile.X;
                newTile.Y = oldTile.Y;

                Add(newTile);
            }
        }
    }
}
