using SRogue.Core.Common.Items;
using SRogue.Core.Common.Items.Concrete;
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
        public int Depth { get; set; }
        public Inventory Inventory { get; set; }
        public int Gold { get; set; }
        public CityShop Shop { get; set; }

        public State()
        {
            Shop = new CityShop();
            Inventory = new Inventory();

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
            public enum Options
            {
                Training,
                Story,
                Exit,
            }

            public string Message;

            public List<string> Story = new List<string>();

            public Options CurrentOption = Options.Exit;

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
                if (CurrentOption == Options.Exit)
                    CurrentOption = Options.Story;
                else if (CurrentOption == Options.Story)
                    CurrentOption = Options.Training;
                else if (CurrentOption == Options.Training)
                    CurrentOption = Options.Exit;
            }

            public void SelectPrev()
            {
                if (CurrentOption == Options.Story)
                    CurrentOption = Options.Exit;
                else if (CurrentOption == Options.Training)
                    CurrentOption = Options.Story;
                else if (CurrentOption == Options.Exit)
                    CurrentOption = Options.Training;
            }

            public void ActivateSelected()
            {
                switch (CurrentOption)
                {
                    case Options.Training:
                        if (GameState.Current.Gold > 100)
                        {
                            GameState.Current.Gold -= 100;
                            GameManager.Current.Player.Attack++;
                            GameManager.Current.Player.HealthMax += 10;
                            Message = "FEEL THE POWER! *you feeling yourself stronger*";
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
                        GameManager.Current.ShopOpened = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
