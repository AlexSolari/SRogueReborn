using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Bases
{
    public class ArmorBase : EquipmentBase
    {
        protected int BaseArmor { get; set; }

        protected int BaseResist { get; set; }

        public int Armor
        {
            get
            {
                switch (Material)
                {
                    case ItemMaterial.Wooden:
                    case ItemMaterial.Iron:
                    case ItemMaterial.Steel:
                        return BaseArmor + (int)Material * 2 + (int)Quality / 2;
                    case ItemMaterial.Diamond:
                    case ItemMaterial.Torium:
                        return BaseArmor + ((int)Material * 3 + (int)Quality * 3) / 2;
                    case ItemMaterial.Glass:
                    case ItemMaterial.Golden:
                    default:
                        return 0;
                }
            }
            set
            {
                BaseArmor = value;
            }
        }

        public int MagicResist
        {
            get
            {
                switch (Material)
                {
                    case ItemMaterial.Glass:
                    case ItemMaterial.Golden:
                        return BaseResist + (int)Material / 2 + (int)Quality * 2;
                    case ItemMaterial.Diamond:
                    case ItemMaterial.Torium:
                        return BaseResist + ((int)Material * 3 + (int)Quality * 3) / 2;
                    case ItemMaterial.Wooden:
                    case ItemMaterial.Iron:
                    case ItemMaterial.Steel:
                    default:
                        return 0;
                }
            }
            set
            {
                BaseResist = value;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name + " ({0} arm, {1} res)".FormatWith(Armor, MagicResist);   
            }
        }

        public ArmorBase(int armor, int resist, ItemType slot)
            : base()
        {
            BaseArmor = armor;
            BaseResist = resist;
            Slot = slot;
        }
    }
}
