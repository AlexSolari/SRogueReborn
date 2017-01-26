using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRogue.Core.Common;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class ItemShop : Unit, IInteractable
    {
        public void Interact(IUnit initiator)
        {
            GameState.Current.ShopOpened = true;
            GameState.Current.ActivateShop();
        }

        public override void Damage(float pure, DamageType type, IEntity source = null)
        {
            return;
        }
    }
}
