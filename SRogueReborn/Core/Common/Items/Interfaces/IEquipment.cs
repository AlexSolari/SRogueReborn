using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Interfaces
{
    public interface IEquipment
    {
        ItemType Slot { get; set; }

        ItemMaterial Material { get; set; }

        ItemQuality Quality { get; set; }
    }
}
