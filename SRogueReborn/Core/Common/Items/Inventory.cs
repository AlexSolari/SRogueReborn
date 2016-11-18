using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Common.Items.Concrete;
using SRogue.Core.Common.Items.Interfaces;
using SRogue.Core.Common.Items.Slots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items
{
    public class Inventory
    {
        public ArmorSlot<Helmet> Head { get; set; } = new ArmorSlot<Helmet>();
        public ArmorSlot<Armor> Chest { get; set; } = new ArmorSlot<Armor>();
        public ArmorSlot<Leggins> Legs { get; set; } = new ArmorSlot<Leggins>();
        public ArmorSlot<Boots> Foot { get; set; } = new ArmorSlot<Boots>();
        public WeaponSlot Weapon { get; set; } = new WeaponSlot();

        public IList<IActivatable> Backpack { get; set; } = new List<IActivatable>();

        public IActivatable Selected = null;
        public int Size;

        public Inventory(int size)
        {
            Size = size;
        }
        
        public void SelectNext()
        {
            if (Selected == null)
            {
                Selected = Backpack.FirstOrDefault();
            }
            else if (Selected != Backpack.LastOrDefault())
            {
                Selected = Backpack[Backpack.IndexOf(Selected) + 1];
            }
            else
            {
                Selected = Backpack.FirstOrDefault();
            }
        }

        public void SelectPrev()
        {
            if (Selected == null)
            {
                Selected = Backpack.LastOrDefault();
            }
            else if (Selected != Backpack.FirstOrDefault())
            {
                Selected = Backpack[Backpack.IndexOf(Selected) - 1];
            }
            else
            {
                Selected = Backpack.LastOrDefault();
            }
        }

        public void ActivateSelected()
        {
            if (Selected == null)
                return;

            Selected.Activate();
        }

        public void SellSelected()
        {
            if (Selected == null)
                return;

            if (Selected is IEquipment)
            {
                var selectedEquipment = Selected as IEquipment;
                GameState.Current.Gold += Math.Max(1, 15 + (int)selectedEquipment.Material * 2 + (int)selectedEquipment.Quality * 3);
            }
            else if (Selected is HealingPotion)
            {
                GameState.Current.Gold += Rnd.Current.Next(2,5) + 20 + (int)Math.Pow(5, (Selected as HealingPotion).Power);
            }
            
            Backpack.Remove(Selected);
            Deselect();
            Selected = Backpack.FirstOrDefault();
        }

        public void Deselect()
        {
            Selected = null;
        }

        public int SummarizeArmor()
        {
            return Head.Item.Armor
                 + Chest.Item.Armor
                 + Legs.Item.Armor
                 + Foot.Item.Armor;
        }

        public int SummarizeResist()
        {
            return Head.Item.MagicResist
                 + Chest.Item.MagicResist
                 + Legs.Item.MagicResist
                 + Foot.Item.MagicResist;
        }
    }
}
