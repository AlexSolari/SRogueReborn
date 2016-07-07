using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common
{
    public class Assets
    {
        /***Tiles***/
        public const char Wall = '█';
        public const char Floor = '.';
        public const char Exit = 'H';
        public const char SpikeTrap = Floor;
        public const char SpikeTrap_Active = '┴';

        /***Entities***/
        public const char Player = '@';
        public const char Zombie = 'Z';
    }
}
