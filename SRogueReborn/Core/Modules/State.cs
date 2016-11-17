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
        public bool InventoryOpened { get; set; } = false;
        public bool ShopOpened { get; set; } = false;
        public bool PopupOpened { get; set; } = false;
        public string PopupMessage { get; set; }
        public Player Player { get; set; }

        public int Depth { get; set; } = 0;
        public Inventory Inventory { get; set; } = new Inventory();
        public int Gold { get; set; } = 0;
        public CityShop Shop { get; set; } = new CityShop();
        public int TrainingLevel { get; set; } = 1;

        public State()
        {
            var sword = new Sword();
            sword.Quality = ItemQuality.Good;
            sword.Material = ItemMaterial.Wooden;
            Inventory.Weapon.Equip(sword);

            var helmet = new Helmet();
            helmet.Quality = 0;
            helmet.Material = 0;
            helmet.Armor = 0;
            helmet.MagicResist = 0;
            helmet.isEmpty = true;
            Inventory.Head.Equip(helmet);

            var armor = new Armor();
            armor.Quality = 0;
            armor.Material = 0;
            armor.Armor = 0;
            armor.MagicResist = 0;
            armor.isEmpty = true;
            Inventory.Chest.Equip(armor);

            var legs = new Leggins();
            legs.Quality = 0;
            legs.Material = 0;
            legs.Armor = 0;
            legs.MagicResist = 0;
            legs.isEmpty = true;
            Inventory.Legs.Equip(legs);

            var boots = new Boots();
            boots.Quality = 0;
            boots.Material = 0;
            boots.Armor = 0;
            boots.MagicResist = 0;
            boots.isEmpty = true;
            Inventory.Foot.Equip(boots);
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
                    CurrentOption = Options.Training;
                else if (CurrentOption.Equals(Options.Training))
                    CurrentOption = Options.Exit;
            }

            public void SelectPrev()
            {
                if (CurrentOption.Equals(Options.Story))
                    CurrentOption = Options.Exit;
                else if (CurrentOption.Equals(Options.Training))
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
                    default:
                        break;
                }
            }
        }
    }
}
