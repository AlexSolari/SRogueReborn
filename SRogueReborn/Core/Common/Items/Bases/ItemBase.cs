using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Bases
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }

        public virtual void OnPickup()
        {
            UiManager.Current.Actions.Append("You picked up '{0}'. ".FormatWith(this.Name));
        }
    }
}
