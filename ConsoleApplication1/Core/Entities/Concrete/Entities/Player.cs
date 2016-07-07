using SRogue.Core.Common.Buffs;
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
            if (target is IHostile && target is IDamageble)
            {
                UiManager.Current.Actions.Append("Dealead {0} damage to Zombie. ".FormatWith(Attack));
                (target as IDamageble).Damage(Attack, Common.DamageType.Physical);
            }
        }
    }
}
