using SRogue.Core.Common;
using SRogue.Core.Common.Buffs;
using SRogue.Core.Common.Items;
using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Entities.Concrete.Entities.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class Player : Unit, IControllable
    {
        [XmlIgnore]
        public List<BuffBase> Buffs { get; set; }

        public Player()
        {
            Buffs = new List<BuffBase>();
        }

        public void Interact(IInteractable target)
        {
            if (target is HostileUnitBase)
            {
                var weapon = GameState.Current.Inventory.Weapon.Item as WeaponBase;
                var targetUnit = target as HostileUnitBase;
                var damage = Attack + weapon.Damage;
                UiManager.Current.Actions.Append("Dealead {0} damage to Zombie. ".FormatWith(damage));
                targetUnit.Damage(damage, Common.DamageType.Physical);
                if (targetUnit.Health <= 0)
                {
                    GameState.Current.Gold += (int)(targetUnit.Reward * (Rnd.Current.NextDouble() + 0.5f));
                }
            }
            if (target is DropUnitBase)
            {
                var targetUnit = target as DropUnitBase;
                targetUnit.GiveItem();
            }
        }
    }
}
