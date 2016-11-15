using SRogue.Core.Common;
using SRogue.Core.Common.Items.Bases;
using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SRogue.Core.Entities.Concrete.Entities.Bases
{
    public abstract class DropUnitBase : Unit, IDropContainer
    {
        [XmlIgnore]
        public IList<ItemBase> Droplist { get; set; } = new List<ItemBase>();

        public bool Picked { get; set; }

        public void GiveItem()
        {
            if (!Picked)
            {
                var item = Droplist[Rnd.Current.Next(Droplist.Count)];
                Kill();
                GameState.Current.Inventory.Backpack.Add(item);
                UiManager.Current.Actions.Append("You picked up '{0}'. ".FormatWith(item.Name));
                Picked = true;
            }
        }

        public void Interact(IUnit initiator)
        {
            GiveItem();
        }
    }
}
