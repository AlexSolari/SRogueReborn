using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common
{
    public class SizeConstants
    {
        public const int TotalScreenWidth = 80;
        public const int TotalScreenHeight = 27;

        public const int GameScreenWidth = TotalScreenWidth - 1;
        public const int GameScreenHeight = TotalScreenHeight - 2;

        public const int FieldWidth = 59;
        public const int FieldHeight = GameScreenHeight;

        public const int UiWidth = 20;
        public const int UiHeight = GameScreenHeight;

        public const int UiMarginWidth = FieldWidth;
        public const int UiMarginHeight = 0;

        public const int InventoryWidth = FieldWidth;
        public const int InventoryHeight = GameScreenHeight - 2;

    }
}
