using SRogue.Core.Common.Items;
using SRogue.Core.Common.Items.Concrete;
using SRogue.Core.Entities.Concrete.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class State
    {
        public const int InventorySize = 9;

        public bool InventoryOpened { get; set; } = false;
        public bool ShopOpened { get; set; } = false;
        public bool PopupOpened { get; set; } = false;
        public string PopupMessage { get; set; }
        public Player Player { get; set; }

        public int Depth { get; set; } = 0;
        public Inventory Inventory { get; set; } = new Inventory(InventorySize);
        public int Gold { get; set; } = 0;
        public CityShop Shop { get; set; } = new CityShop();
        public int TrainingLevel { get; set; } = 1;

        public State()
        {
            Player = EntityLoadManager.Current.Load<Player>();

            var sword = new Sword();
            sword.Quality = ItemQuality.Good;
            sword.Material = ItemMaterial.Wooden;
            Inventory.Weapon.Equip(sword);

            var wand = new Wand();
            sword.Quality = ItemQuality.Good;
            sword.Material = ItemMaterial.Iron;
            Inventory.Backpack.Add(wand);

            Inventory.Head.Equip(null);
            Inventory.Chest.Equip(null);
            Inventory.Legs.Equip(null);
            Inventory.Foot.Equip(null);

            for (int i = 0; i < 3; i++)
            {
                Inventory.Backpack.Add(new HealingPotion());
            }
        }

        public void ActivateShop()
        {
            Shop.Load(Depth);
            DisplayManager.Current.SaveOverlay();
        }

        public class CityShop
        {
            public class Options
            {
                public const string Training = "Training ({0} GOLD)";
                public const string Healing = "Healing Potion (40 GOLD)";
                public const string Story = "Story";
                public const string Exit = "Exit";
            }

            public string Message { get; set; }

            public List<string> Story { get; set; } = new List<string>();

            public string CurrentOption { get; set; } = Options.Exit;

            public void Load(int depth)
            {
                Story.Clear();
                Message = "Welcome to CAMP {0}".FormatWith(depth / 5);
                var path = "res/story/{0}.txt".FormatWith(depth);
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine();
                        Story.Add(row);
                    }
                }
            }

            public void SelectNext()
            {
                if (CurrentOption.Equals(Options.Exit))
                    CurrentOption = Options.Story;
                else if (CurrentOption.Equals(Options.Story))
                    CurrentOption = Options.Healing;
                else if (CurrentOption.Equals(Options.Healing))
                    CurrentOption = Options.Training;
                else if (CurrentOption.Equals(Options.Training))
                    CurrentOption = Options.Exit;
            }

            public void SelectPrev()
            {
                if (CurrentOption.Equals(Options.Story))
                    CurrentOption = Options.Exit;
                else if (CurrentOption.Equals(Options.Training))
                    CurrentOption = Options.Healing;
                else if (CurrentOption.Equals(Options.Healing))
                    CurrentOption = Options.Story;
                else if (CurrentOption.Equals(Options.Exit))
                    CurrentOption = Options.Training;
            }

            public void ActivateSelected()
            {
                switch (CurrentOption)
                {
                    case Options.Training:
                        if (GameState.Current.Gold > Math.Pow(2, GameState.Current.TrainingLevel))
                        {
                            GameState.Current.Gold -= (int)Math.Pow(2, GameState.Current.TrainingLevel);
                            GameState.Current.TrainingLevel++;
                            GameState.Current.Player.Attack++;
                            GameState.Current.Player.HealthMax += 5;
                            Message = "*you feeling yourself stronger* (+1 DMG, +5 HP)";
                        }
                        else
                        {
                            Message = "You can't efford this.";
                        }
                        break;
                    case Options.Story:
                        Message = string.Join("#", Story);
                        break;
                    case Options.Exit:
                        DisplayManager.Current.LoadOverlay();
                        GameState.Current.ShopOpened = false;
                        break;
                    case Options.Healing:
                        if (GameState.Current.Gold > 40)
                        {
                            if (GameState.Current.Inventory.Backpack.Count < GameState.Current.Inventory.Size)
                            {
                                GameState.Current.Gold -= 40;
                                var potion = new HealingPotion();
                                var power = ((GameState.Current.Depth / 5) - 1 ) / 3;
                                potion.Power = power;
                                GameState.Current.Inventory.Backpack.Add(potion);
                                Message = "'{0}' added to your inventory".FormatWith(potion.Name);
                            }
                            else
                            {
                                Message = "Your inventory full.";
                            }
                        }
                        else
                        {
                            Message = "You can't efford this.";
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
