using SRogue.Core.Common;
using SRogue.Core.Common.Items.Concrete;
using SRogue.Core.Common.TickEvents;
using SRogue.Core.Common.TickEvents.Bases;
using SRogue.Core.Common.World;
using SRogue.Core.Common.World.Generation;
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
        protected Generator Generator { get; set; } = new Generator();
        public List<TickEventBase> OnTickEndEvents { get; set; } = new List<TickEventBase>();
        public Dictionary<ConsoleKey, Func<bool>> UsualControl { get; set; }
        public Dictionary<ConsoleKey, Action> InventoryControl { get; set; }
        public Dictionary<ConsoleKey, Action> ShopControl { get; set; }

        public Game()   
        {
            UsualControl = new Dictionary<ConsoleKey, Func<bool>> // if returns true, then call GameTick after action
            {
                [ConsoleKey.W] = () => 
                {
                    if (GameState.Current.DirectionSelect)
                        GameState.Current.Player.UseTargetedAbility(Direction.Top);
                    else
                        GameState.Current.Player.Move(Direction.Top);
                    return true;
                },
                [ConsoleKey.S] = () => 
                {
                    if (GameState.Current.DirectionSelect)
                        GameState.Current.Player.UseTargetedAbility(Direction.Bottom);
                    else
                        GameState.Current.Player.Move(Direction.Bottom);
                    return true;
                },
                [ConsoleKey.A] = () => 
                {
                    if (GameState.Current.DirectionSelect)
                        GameState.Current.Player.UseTargetedAbility(Direction.Left);
                    else
                        GameState.Current.Player.Move(Direction.Left);
                    return true;
                },
                [ConsoleKey.D] = () => 
                {
                    if (GameState.Current.DirectionSelect)
                        GameState.Current.Player.UseTargetedAbility(Direction.Right);
                    else
                        GameState.Current.Player.Move(Direction.Right);
                    return true;
                },
                [ConsoleKey.E] = () => { GameState.Current.Player.Examine(); return true; },
                [ConsoleKey.I] = () => { ToggleInventoryPopup(); return false; },
                [ConsoleKey.X] = () => 
                {
                    if (GameState.Current.Inventory.Weapon.Item is Wand)
                    {

                        UiManager.Current.Actions.Append("Select direction");
                        GameState.Current.DirectionSelect = true;
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
                [ConsoleKey.I] = () => ToggleInventoryPopup(),
            };

            ShopControl = new Dictionary<ConsoleKey, Action>
            {
                [ConsoleKey.W] = () => GameState.Current.Shop.SelectNext(),
                [ConsoleKey.S] = () => GameState.Current.Shop.SelectPrev(),
                [ConsoleKey.Q] = () => GameState.Current.Shop.ActivateSelected(),
            };
        }

        public void GenerateWorld()
        {
            GameState.Current.Depth++; 
            GameState.Current.CurrentLevel = Generator.GenerateWorld();
        }
        
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
                    if (input == ConsoleKey.E || GameState.Current.BlastFired)
                    {
                        DisplayManager.Current.Draw();
                        Thread.Sleep(333);
                        redrawActions = false;
                        GameState.Current.BlastFired = false;
                        DisplayManager.Current.BlastedPoints.Clear();
                    }
                    
                }

                if (needToTick)
                    GameTick();
            }

            return redrawActions;
        }

        private void ToggleInventoryPopup()
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
            var enitities = GameState.Current.CurrentLevel.Entities.OrderBy(x => (x is Player));
            foreach (var entity in enitities)
            {
                if (entity is IAiControllable)
                {
                    (entity as IAiControllable).AiTick();
                }

                var tileUnderEntity = GameState.Current.CurrentLevel.GetTileAt(entity.X, entity.Y);

                tileUnderEntity.OnStep(entity);

                if (entity.Health < entity.HealthMax)
                {
                    var regen = (entity is IHostile) ? GameplayConstants.UndeadHeathRegenerationRate : GameplayConstants.HeroHeathRegenerationRate;
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
    }
}
