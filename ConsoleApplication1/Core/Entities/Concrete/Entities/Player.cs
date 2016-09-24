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
                var damage = SummarizeAttack();
                UiManager.Current.Actions.Append("Dealead {0} damage to {1}. ".FormatWith(damage, target.GetType().Name));
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

        public virtual int SummarizeAttack()
        {
            return Attack + (GameState.Current.Inventory.Weapon.Item as WeaponBase).Damage;
        }

        public override int SummarizeArmor()
        {
            var inv = GameState.Current.Inventory;
            return base.SummarizeArmor() 
                + (inv.Head.Item as ArmorBase).Armor 
                + (inv.Chest.Item as ArmorBase).Armor 
                + (inv.Legs.Item as ArmorBase).Armor 
                + (inv.Foot.Item as ArmorBase).Armor;
        }

        public override int SummarizeResist()
        {
            var inv = GameState.Current.Inventory;
            return base.SummarizeResist() 
                + (inv.Head.Item as ArmorBase).MagicResist
                + (inv.Chest.Item as ArmorBase).MagicResist
                + (inv.Legs.Item as ArmorBase).MagicResist
                + (inv.Foot.Item as ArmorBase).MagicResist;
        }

        public void Examine()
        {
            var tiles = GetNearbyTiles<IActivatable>();
            
            if (tiles.Count() == 0)
            {
                UiManager.Current.Actions.Append("Nothing found. ");
            }

            foreach (var tile in tiles)
            {
                tile.Activate();
                UiManager.Current.Actions.Append("Found {0}. ".FormatWith(tile.GetType().Name));
            }
        }

        protected IEnumerable<TType> GetNearbyTiles<TType>()
            where TType : class
        {
            return GameManager.Current.Tiles.Where(t => t.X.IsInRange(X - 1, X + 1) && t.Y.IsInRange(Y - 1, Y + 1) && t is TType).Select(t => t as TType);
        }
    }
}
