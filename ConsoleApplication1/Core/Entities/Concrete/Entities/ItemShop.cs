using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Concrete.Entities
{
    public class ItemShop : Unit, IInteractable
    {
        public void Interact(IUnit initiator)
        {
            GameManager.Current.ShopOpened = true;
            GameState.Current.ActivateShop();
        }
    }
}
