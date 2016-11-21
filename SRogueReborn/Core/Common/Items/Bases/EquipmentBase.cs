using SRogue.Core.Common.Items.Concrete;
using SRogue.Core.Common.Items.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Bases
{
    public abstract class EquipmentBase : ItemBase, IEquipment, IActivatable
    {
        public ItemType Slot { get; set; }

        public ItemMaterial Material { get; set; }

        public ItemQuality Quality { get; set; }

        public override string Name
        {
            get
            {
                return "{0} {1} {2}".FormatWith(Quality, Material, this.GetType().Name);
            }
        }

        public override bool OnPickup()
        {
            if (GameState.Current.Inventory.Backpack.Count < GameState.Current.Inventory.Size) { 
                GameState.Current.Inventory.Backpack.Add(this);
                return base.OnPickup();
            }

            UiManager.Current.Actions.Append("Inventory full. ");
            return false;
        }

        public void Activate()
        {
            GameState.Current.Inventory.Backpack.Remove(this);
            switch (Slot)
            {
                case ItemType.Head:
                    if (GameState.Current.Inventory.Head.Item != null)
                        GameState.Current.Inventory.Backpack.Add(GameState.Current.Inventory.Head.Dequip());
                    GameState.Current.Inventory.Head.Equip((Helmet)this);
                    break;
                case ItemType.Chest:
                    if (GameState.Current.Inventory.Chest.Item != null)
                        GameState.Current.Inventory.Backpack.Add(GameState.Current.Inventory.Chest.Dequip());
                    GameState.Current.Inventory.Chest.Equip((Armor)this);
                    break;
                case ItemType.Legs:
                    if (GameState.Current.Inventory.Legs.Item != null)
                        GameState.Current.Inventory.Backpack.Add(GameState.Current.Inventory.Legs.Dequip());
                    GameState.Current.Inventory.Legs.Equip((Leggins)this);
                    break;
                case ItemType.Foot:
                    if (GameState.Current.Inventory.Foot.Item != null)
                        GameState.Current.Inventory.Backpack.Add(GameState.Current.Inventory.Foot.Dequip());
                    GameState.Current.Inventory.Foot.Equip((Boots)this);
                    break;
                case ItemType.Weapon:
                    if (GameState.Current.Inventory.Weapon.Item != null)
                        GameState.Current.Inventory.Backpack.Add(GameState.Current.Inventory.Weapon.Dequip());
                    GameState.Current.Inventory.Weapon.Equip((WeaponBase)this);
                    break;
                default:
                    break;
            }
            GameState.Current.Inventory.Deselect();
            GameState.Current.Inventory.SelectNext();
        }

        public EquipmentBase()
        {
            var possibleMaterials = Enum.GetValues(typeof(ItemMaterial));
            Material = (ItemMaterial)possibleMaterials.GetValue(Rnd.Current.Next(possibleMaterials.Length));

            var possibleQualitys = Enum.GetValues(typeof(ItemQuality));
            Quality = (ItemQuality)possibleQualitys.GetValue(Rnd.Current.Next(possibleQualitys.Length));
        }
    }
}
