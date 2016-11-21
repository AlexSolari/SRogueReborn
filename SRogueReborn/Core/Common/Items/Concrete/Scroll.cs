using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Common.Items.Interfaces;
using SRogue.Core.Common.TickEvents;
using SRogue.Core.Entities.Concrete.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Scroll : ItemBase, IActivatable
    {
        public int Type { get; set; }

        public override string Name
        {
            get
            {
                return "Scroll of {0}".FormatWith(Prefix);
            }
        }

        public string Prefix { get; private set; }

        public void Activate()
        {
            GameState.Current.Inventory.Backpack.Remove(this);

            switch (Type)
            {
                case 0:
                    var equiped = GameState.Current.Inventory.Equiped;
                    foreach (var item in equiped)
                    {
                        item.Quality = (ItemQuality)Math.Min((int)item.Quality + 1, (int)ItemQuality.Godly);
                    }
                    break;
                case 1:
                    GameManager.Current.OnTickEndEvents.Add(new EventStoneSkin(GameState.Current.Player));
                    break;
                case 2:
                    GameManager.Current.OnTickEndEvents.Add(new EventRegenerationHealing(GameState.Current.Player));
                    break;
                default:
                    break;
            }

            GameState.Current.Inventory.Deselect();
            GameState.Current.Inventory.SelectNext();
        }

        public override bool OnPickup()
        {
            if (GameState.Current.Inventory.Backpack.Count < GameState.Current.Inventory.Size)
            {
                GameState.Current.Inventory.Backpack.Add(this);
                return base.OnPickup();
            }

            UiManager.Current.Actions.Append("Inventory full. ");
            return false;
        }

        public Scroll()
        {
            Type = Rnd.Current.Next(0, 3);

            switch (Type)
            {
                case 0:
                    Prefix = "Upgrade";
                    break;
                case 1:
                    Prefix = "Stoneskin";
                    break;
                case 2:
                    Prefix = "Regeneration";
                    break;
                default:
                    break;
            }
        }
    }
}
