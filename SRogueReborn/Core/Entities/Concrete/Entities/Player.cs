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
        public List<BuffBase> Buffs { get; set; } = new List<BuffBase>();
        
        public void Interact(IInteractable target)
        {
            target.Interact(this);
        }

        public override int SummarizeAttack()
        {
            return Attack + (GameState.Current.Inventory.Weapon.Item as WeaponBase).Damage;
        }

        public override int SummarizeArmor()
        {
            return base.SummarizeArmor() + GameState.Current.Inventory.SummarizeArmor();
        }

        public override int SummarizeResist()
        {
            return base.SummarizeResist() + GameState.Current.Inventory.SummarizeResist();
        }

        public void Examine()
        {
            var tiles = GetNearbyTiles<IActivatable>();
            
            if (!tiles.Any())
            {
                UiManager.Current.Actions.Append("Nothing found. ");
            }

            foreach (var tile in tiles)
            {
                tile.Activate();
                UiManager.Current.Actions.Append("Found {0}. ".FormatWith(tile.GetType().Name));
            }

            DisplayManager.Current.ExaminatedPoint = new Point() { X = X, Y = Y };
        }

        protected IEnumerable<TType> GetNearbyTiles<TType>()
            where TType : class
        {
            return GameManager.Current.Tiles.Where(t => t.X.IsInRange(X - 1, X + 1) && t.Y.IsInRange(Y - 1, Y + 1) && t is TType).Select(t => t as TType);
        }
    }
}
