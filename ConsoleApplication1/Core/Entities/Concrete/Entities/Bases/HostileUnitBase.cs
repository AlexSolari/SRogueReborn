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
    }
}
