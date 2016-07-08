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
        public IList<ItemBase> Droplist { get; set; }

        public DropUnitBase()
        {
            Droplist = new List<ItemBase>();
        }

        public void GiveItem()
        {
            var item = Droplist[Rnd.Current.Next(Droplist.Count)];
            Kill();
            GameState.Current.Inventory.Backpack.Add(item);
            UiManager.Current.Actions.Append("You picked up '{0}'. ".FormatWith(item.Name));
        }
    }
}
