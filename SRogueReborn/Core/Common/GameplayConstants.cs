using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common
{
    public class GameplayConstants
    {
        public const float PhysicalDamageDecreaseComponent = 0.98f;
        public const float PureDamageDecreaseComponent = 1f;
        public const float MagicalDamageDecreaseComponent = 0.95f;

        public const float UndeadHeathRegenerationRate = 0.33f;
        public const float HeroHeathRegenerationRate = 1f;
    }
}
