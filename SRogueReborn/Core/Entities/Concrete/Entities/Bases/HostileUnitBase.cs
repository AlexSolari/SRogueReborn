using SRogue.Core.Common;
using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Concrete.Entities.Bases
{
    public abstract class HostileUnitBase : Unit, IAiControllable, IHostile
    {
        public int Reward { get; set; }

        public virtual void AiTick()
        {
            AiManager.Current.Execute(this.GetType(), this);
        }

        public void Interact(IUnit initiator)
        {
            if (this is IDodger && Rnd.Current.NextDouble() > ((IDodger)this).DodgeChance)
            {
                UiManager.Current.Actions.Append("{0} dodged. ".FormatWith(this.GetType().Name));
                return;
            }
            var weapon = GameState.Current.Inventory.Weapon.Item as WeaponBase;
            var targetUnit = this;
            var damage = initiator.SummarizeAttack();
            UiManager.Current.Actions.Append("Dealed {0} damage to {1}. ".FormatWith(damage, this.GetType().Name));
            targetUnit.Damage(damage, DamageType.Physical, initiator);
        }

        public override void Kill(IEntity source = null)
        {
            if (source == GameState.Current.Player)
                GameState.Current.Gold += (int)(Reward * (Rnd.Current.NextDouble() + 0.5f));

            base.Kill();
        }
    }
}
