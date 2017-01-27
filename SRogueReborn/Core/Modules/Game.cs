﻿using SRogue.Core.Common;
using SRogue.Core.Common.Items.Concrete;
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
        public ITile[,] Tiles { get; private set; } = new ITile[25, 59];
        public List<TickEventBase> OnTickEndEvents { get; set; } = new List<TickEventBase>();

        public Dictionary<ConsoleKey, Func<bool>> UsualControl { get; set; }
        public Dictionary<ConsoleKey, Action> InventoryControl { get; set; }
        public Dictionary<ConsoleKey, Action> ShopControl { get; set; }

        public bool DirectionSelect { get; set; }
        public bool BlastFired { get; set; }

        public Game()   
        {
            UsualControl = new Dictionary<ConsoleKey, Func<bool>> // if returns true, then call GameTick after action
            {
                [ConsoleKey.W] = () => 
                {
                    if (DirectionSelect)
                        UseTargetedAbility(Direction.Top);
                    else
                        GameState.Current.Player.Move(Direction.Top);
                    return true;
                },
                [ConsoleKey.S] = () => 
                {
                    if (DirectionSelect)
                        UseTargetedAbility(Direction.Bottom);
                    else
                        GameState.Current.Player.Move(Direction.Bottom);
                    return true;
                },
                [ConsoleKey.A] = () => 
                {
                    if (DirectionSelect)
                        UseTargetedAbility(Direction.Left);
                    else
                        GameState.Current.Player.Move(Direction.Left);
                    return true;
                },
                [ConsoleKey.D] = () => 
                {
                    if (DirectionSelect)
                        UseTargetedAbility(Direction.Right);
                    else
                        GameState.Current.Player.Move(Direction.Right);
                    return true;
                },
                [ConsoleKey.E] = () => { GameState.Current.Player.Examine(); return true; },
                [ConsoleKey.I] = () => { ToggleInventory(); return false; },
                [ConsoleKey.X] = () => 
                {
                    if (GameState.Current.Inventory.Weapon.Item is Wand)
                    {

                        UiManager.Current.Actions.Append("Select direction");
                        DirectionSelect = true;
                        return false;
                    }

                    return true;
                },
            };

            InventoryControl = new Dictionary<ConsoleKey, Action>
            {
                [ConsoleKey.W] = () => GameState.Current.Inventory.SelectNext(),
                [ConsoleKey.S] = () => GameState.Current.Inventory.SelectPrev(),
                [ConsoleKey.Q] = () => GameState.Current.Inventory.ActivateSelected(),
                [ConsoleKey.E] = () => GameState.Current.Inventory.SellSelected(),
                [ConsoleKey.I] = () => ToggleInventory(),
            };

            ShopControl = new Dictionary<ConsoleKey, Action>
            {
                [ConsoleKey.W] = () => GameState.Current.Shop.SelectNext(),
                [ConsoleKey.S] = () => GameState.Current.Shop.SelectPrev(),
                [ConsoleKey.Q] = () => GameState.Current.Shop.ActivateSelected(),
            };
        }

        public void UseTargetedAbility(Direction direction)
        {
            GameState.Current.Inventory.Weapon.Ability(direction);
            DirectionSelect = false;
        }



        #region GameObjects

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

            OnTickEndEvents.Add(new EventTileRemove(oldtile));

            Tiles[tile.Y, tile.X] = tile;
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
                var needToTick = true;
                if (UsualControl.ContainsKey(input))
                {
                    needToTick = UsualControl[input]();
                    if (input == ConsoleKey.E || BlastFired)
                    {
                        DisplayManager.Current.Draw();
                        Thread.Sleep(333);
                        redrawActions = false;
                        BlastFired = false;
                        DisplayManager.Current.BlastedPoints.Clear();
                    }
                    
                }

                if (needToTick)
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
            Ai.Container.RecalculatePlayerPathmap();
            var enitities = Entities.OrderBy(x => (x is Player));
            foreach (var entity in enitities)
            {
                if (entity is IAiControllable)
                {
                    (entity as IAiControllable).AiTick();
                }

                var tileUnderEntity = GetTileAt(entity.X, entity.Y);

                tileUnderEntity.OnStep(entity);

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

        #endregion

        #region WorldGenerator
       
        public void GenerateWorld()
        {
            GameState.Current.Depth++;
            var isCity = GameState.Current.Depth % 5 == 0 && GameState.Current.Depth <= 35;
            var isBoss = !isCity && GameState.Current.Depth % 5 == 4 || GameState.Current.Depth > 35;

            Tiles = new ITile[25, 59];
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
            else
            {
                AddGoldPickups();
            }

            if (isBoss)
            {
                GameState.Current.PopupMessage = "You feel evil presense of powerful creature.";
                GameState.Current.PopupOpened = true;
            }

            GenerateExit(centers);
            DisplayManager.Current.ResetOverlay();

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

        private void AddGoldPickups()
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

        private void AddPlayer(IList<Point> centers)
        {
            var player = GameState.Current.Player = GameState.Current.Player;
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
