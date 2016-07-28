using SRogue.Core.Common.Items;
using SRogue.Core.Common.Items.Concrete;
using System;
using System.Collections.Generic;
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

        public State()
        {
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
    }
}
