using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Common.Items.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class HealingPotion : ItemBase, IActivatable
    {
        public int Power { get; set; }

        public override string Name
        {
            get
            {
                return "{0}Healing Potion".FormatWith(Prefix);
            }
        }

        public string Prefix { get; private set; }

        public void Activate()
        {
            GameState.Current.Inventory.Backpack.Remove(this);
            var healing = 20 + 20 * Power;
            GameState.Current.Player.Health = Math.Min(GameState.Current.Player.HealthMax, GameState.Current.Player.Health + healing);
            GameState.Current.Inventory.Deselect();
            GameState.Current.Inventory.SelectNext();
        }

        public HealingPotion()
        {
            Power = Rnd.Current.Next(0, 3);

            switch (Power)
            {
                case 0:
                    Prefix = "Lesser ";
                    break;
                case 1:
                    Prefix = "";
                    break;
                case 2:
                    Prefix = "Greater ";
                    break;
                default:
                    break;
            }
        }
    }
}
