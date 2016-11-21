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
        public IList<IEquipment> Equiped
        {
            get
            {
                var result = new List<IEquipment>();

                result.Add(Head.Item);
                result.Add(Chest.Item);
                result.Add(Legs.Item);
                result.Add(Foot.Item);
                result.Add(Weapon.Item);

                return result;
            }
        }

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

        public void SelectPrev()
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
                GameState.Current.Gold += (int)Math.Floor((Rnd.Current.Next(5, 10) + (1 + (int)selectedEquipment.Material) * 7) * (1 + 0.2*(int)selectedEquipment.Quality));
            }
            else if (Selected is HealingPotion)
            {
                GameState.Current.Gold += Rnd.Current.Next(2,5) + 20 + (int)Math.Pow(5, (Selected as HealingPotion).Power);
            }
            else if (Selected is Scroll)
            {
                GameState.Current.Gold += Rnd.Current.Next(12, 25) + 100;
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
             return Head.Armor
                 + Chest.Armor
                 + Legs.Armor
                 + Foot.Armor;
        }

        public int SummarizeResist()
        {
            return Head.MagicResist
                 + Chest.MagicResist
                 + Legs.MagicResist
                 + Foot.MagicResist;
        }
    }
}
