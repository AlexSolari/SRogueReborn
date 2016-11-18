using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Bases
{
    public abstract class EquipmentBase : ItemBase
    {
        public bool isEmpty { get; set; }

        public ItemType Slot { get; set; }

        public ItemMaterial Material { get; set; }

        public ItemQuality Quality { get; set; }

        public override string Name
        {
            get
            {
                if (isEmpty)
                    return "[ No {0} ]".FormatWith(this.GetType().Name);

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

        public EquipmentBase()
        {
            var possibleMaterials = Enum.GetValues(typeof(ItemMaterial));
            Material = (ItemMaterial)possibleMaterials.GetValue(Rnd.Current.Next(possibleMaterials.Length));

            var possibleQualitys = Enum.GetValues(typeof(ItemQuality));
            Quality = (ItemQuality)possibleQualitys.GetValue(Rnd.Current.Next(possibleQualitys.Length));
        }
    }
}
