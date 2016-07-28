using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items
{
    public class Inventory
    {
        public ItemSlot Head { get; set; }
        public ItemSlot Chest { get; set; }
        public ItemSlot Legs { get; set; }
        public ItemSlot Foot { get; set; }
        public ItemSlot Weapon { get; set; }

        public IList<ItemBase> Backpack { get; set; }

        public ItemBase Selected = null;

        public Inventory()
        {
            Backpack = new List<ItemBase>();

            Head = new ItemSlot(ItemType.Head);
            Chest = new ItemSlot(ItemType.Chest);
            Legs = new ItemSlot(ItemType.Legs);
            Foot = new ItemSlot(ItemType.Foot);
            Weapon = new ItemSlot(ItemType.Weapon);
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

        public void EquipSelected()
        {
            if (Selected == null)
                return;

            Backpack.Remove(Selected);
            switch (Selected.Slot)
            {
                case ItemType.Head:
                    if (Head.Item != null && !Head.Item.isEmpty)
                        Backpack.Add(Head.Dequip());
                    Head.Equip(Selected);
                    break;
                case ItemType.Chest:
                    if (Chest.Item != null && !Chest.Item.isEmpty)
                        Backpack.Add(Chest.Dequip());
                    Chest.Equip(Selected);
                    break;
                case ItemType.Legs:
                    if (Legs.Item != null && !Legs.Item.isEmpty)
                        Backpack.Add(Legs.Dequip());
                    Legs.Equip(Selected);
                    break;
                case ItemType.Foot:
                    if (Foot.Item != null && !Foot.Item.isEmpty)
                        Backpack.Add(Foot.Dequip());
                    Foot.Equip(Selected);
                    break;
                case ItemType.Weapon:
                    if (Weapon.Item != null && !Weapon.Item.isEmpty)
                        Backpack.Add(Weapon.Dequip());
                    Weapon.Equip(Selected);
                    break;
                default:
                    break;
            }
            Deselect();
            SelectNext();
        }

        public void SellSelected()
        {
            if (Selected == null)
                return;

            GameState.Current.Gold += 15 + (int)Selected.Material * 2 + (int)Selected.Quality * 3;
            Backpack.Remove(Selected);
            Deselect();
        }

        public void Deselect()
        {
            Selected = null;
        }
    }
}
