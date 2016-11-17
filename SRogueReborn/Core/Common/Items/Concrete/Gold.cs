using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.Items.Concrete
{
    public class Gold : PickupBase
    {
        public int Count { get; set; }

        public override string Name
        {
            get
            {
                return "{0} Gold".FormatWith(Count);
            }
        }

        public override void OnPickup()
        {
            GameState.Current.Gold += Count;
            base.OnPickup();
        }

        public Gold()
        {
            Count = Rnd.Current.Next(5, 50);
        }
    }
}
