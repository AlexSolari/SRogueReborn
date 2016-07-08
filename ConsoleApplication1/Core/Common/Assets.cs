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
#if DEBUG
        public const char SpikeTrap = '!';
#else
        public const char SpikeTrap = Floor;        
#endif
        public const char SpikeTrap_Active = '┴';

        /***Entities***/
        public const char Player = '@';
        public const char Zombie = 'z';
        public const char ZombieBoss = 'Z';


        public const char Item = 'u';
        public const char RandomSwordDrop = Item;
    }
}
