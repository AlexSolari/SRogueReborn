using SRogue.Core.Common;
using SRogue.Core.Common.TickEvents;
using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Entities;
using SRogue.Core.Entities.Concrete.Entities;
using SRogue.Core.Entities.Concrete.Tiles;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class Game
    {
        public IList<IUnit> Entities { get; private set; } = new List<IUnit>();
        public IList<ITile> Tiles { get; private set; } = new List<ITile>();
        public List<TickEventBase> OnTickEndEvents { get; set; } = new List<TickEventBase>();

        public Dictionary<ConsoleKey, Action> UsualControl { get; set; }
        public Dictionary<ConsoleKey, Action> InventoryControl { get; set; }
        public Dictionary<ConsoleKey, Action> ShopControl { get; set; }

        public Game()   
        {
            UsualControl = new Dictionary<ConsoleKey, Action> {
                [ ConsoleKey.W ] = () => GameState.Current.Player.Move(Direction.Top),
                [ ConsoleKey.S ] = () => GameState.Current.Player.Move(Direction.Bottom),
                [ ConsoleKey.A ] = () => GameState.Current.Player.Move(Direction.Left),
                [ ConsoleKey.D ] = () => GameState.Current.Player.Move(Direction.Right),
                [ ConsoleKey.E ] = () => GameState.Current.Player.Examine(),
            };

            InventoryControl = new Dictionary<ConsoleKey, Action> {
                [ ConsoleKey.W ] = () => GameState.Current.Inventory.SelectNext(),
                [ ConsoleKey.S ] = () => GameState.Current.Inventory.SelectPrev(),
                [ ConsoleKey.Q ] = () => GameState.Current.Inventory.EquipSelected(),
                [ ConsoleKey.E ] = () => GameState.Current.Inventory.SellSelected(),
            };

            ShopControl = new Dictionary<ConsoleKey, Action>
            {
                [ConsoleKey.W] = () => GameState.Current.Shop.SelectNext(),
                [ConsoleKey.S] = () => GameState.Current.Shop.SelectPrev(),
                [ConsoleKey.Q] = () => GameState.Current.Shop.ActivateSelected(),
            };
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

        public bool ProcessInput(ConsoleKey input)
        {
            var redrawActions = true;

            if (GameState.Current.PopupOpened)
            {
                if (input == ConsoleKey.Q)
                {
                    GameState.Current.PopupOpened = false;
                    DisplayManager.Current.LoadOverlay();
                }
                else
                    return redrawActions;
            }

            if (input == ConsoleKey.I)
            {
                ToggleInventory();
            }

            if (GameState.Current.ShopOpened)
            {
                if (ShopControl.ContainsKey(input))
                {
                    ShopControl[input]();
                }
            }
            else if (GameState.Current.InventoryOpened)
            {
                if (InventoryControl.ContainsKey(input))
                {
                    InventoryControl[input]();
                }
            }
            else
            {
                if (UsualControl.ContainsKey(input))
                {
                    UsualControl[input]();
                    if (input == ConsoleKey.E)
                    {
                        DisplayManager.Current.Draw();
                        Thread.Sleep(333);
                        redrawActions = false;
                    }
                    
                }

                GameTick();
            }

            return redrawActions;
        }

        private void ToggleInventory()
        {
            if (GameState.Current.InventoryOpened)
            {
                DisplayManager.Current.LoadOverlay();
                GameState.Current.Inventory.Deselect();
            }
            else
            {
                DisplayManager.Current.SaveOverlay();
                GameState.Current.Inventory.SelectNext();
            }
            GameState.Current.InventoryOpened = !GameState.Current.InventoryOpened;
        }

        public void GameTick()
        {
            var enitities = Entities.OrderBy(x => (x is Player));
            foreach (var entity in enitities)
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
                    var regen = (entity is IHostile) ? 0.33f : 1;
                    entity.Health = Math.Min(entity.Health + regen, entity.HealthMax);
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

        public ITile GetRandomTile(bool pathable = false, bool withoutEntities = true)
        {
            var entities = Entities.ToList();
            var tiles = Tiles.Where(x => (pathable) ? x.Pathable : !x.Pathable).ToList();
            entities.ForEach(t => tiles = tiles.Except(tiles.Where(e => e.X == t.X && e.Y == t.Y)).ToList());
            var tile = tiles[Rnd.Current.Next(tiles.Count())];
            return tile;
        }

        #endregion

        #region WorldGenerator
       
        public void GenerateWorld()
        {
            GameState.Current.Depth++;
            var isCity = GameState.Current.Depth % 5 == 0 && GameState.Current.Depth <= 35;
            var isBoss = !isCity && GameState.Current.Depth % 5 == 4 || GameState.Current.Depth > 35;

            Tiles.Clear();
            Entities.Clear();

            var roomsCount = Rnd.Current.Next(6, 14);

            if (isCity)
                roomsCount = 3;

            var centers = new List<Point>();
            for (int i = 0; i < roomsCount; i++)
            {
                GenerateRoom(centers);
            }

            GenerateCorridors(centers);

            Fill();

            if (!isCity)
            {

                GenerateTraps();

                GenerateEnemies(isBoss);

            }
            AddPlayer(centers);

            if (isCity)
            {
                GameState.Current.PopupMessage = "This place is safe. There is a shop nearby.";
                GameState.Current.PopupOpened = true;
                GenerateCity(centers);
            }

            if (isBoss)
            {
                GameState.Current.PopupMessage = "You feel evil presense of powerful creature.";
                GameState.Current.PopupOpened = true;
            }

            GenerateExit(centers);
#if !DEBUG
            DisplayManager.Current.ResetOverlay();
#endif
            DisplayManager.Current.SaveOverlay();

            if (isBoss)
            {
                MusicManager.Current.Play(Music.Theme.Boss);
            }
            else if (isCity)
            {
                MusicManager.Current.Play(Music.Theme.City);
            }
            else
            {
                MusicManager.Current.Play(Music.Theme.Default);
            }
        }

        private void AddPlayer(IList<Point> centers)
        {
            var player = GameState.Current.Player = GameState.Current.Player ?? EntityLoadManager.Current.Load<Player>();
            player.Health = player.HealthMax;
            player.X = centers.First().X;
            player.Y = centers.First().Y;
            Add(player);
        }

        protected void GenerateExit(IList<Point> centers)
        {
            var exit = EntityLoadManager.Current.Load<Exit>();
            exit.X = centers.Last().X;
            exit.Y = centers.Last().Y;
            Add(exit);
        }

        protected void GenerateCity(IList<Point> centers)
        {
            var shop = EntityLoadManager.Current.Load<ItemShop>();
            shop.X = centers.Skip(1).First().X;
            shop.Y = centers.Skip(1).First().Y;
            Add(shop);
        }

        protected void GenerateEnemies(bool isBoss)
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

            if (GameState.Current.Depth > 1)
            {
                if (Rnd.Current.NextDouble() > 0.5)
                {
                    var count = Rnd.Current.Next(1, 5);
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
