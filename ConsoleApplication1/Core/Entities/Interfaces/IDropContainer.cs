using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Interfaces
{
    public interface IDropContainer : IInteractable
    {
        IList<ItemBase> Droplist { get; set; }
        void GiveItem();
    }
}
