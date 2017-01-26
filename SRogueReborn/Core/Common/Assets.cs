using SRogue.Core.Entities.Interfaces;
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
        public const char Ghost = 'g';

        public const char ItemShop = 'O';
        public const char Item = 'u';
        public const char RandomItemDrop = Item;
        public const char GoldDrop = '8';
        public const char ScrollDrop = 's';

        /***UI***/
        public const char UiBorder = '▓';
        public const char Fog = '▒';
        public const char PlayerVisionMarker = '\0';
        public const char PlayerVision = ' ';
        public const char ExaminatedMarker = '*';
        public const char Blast = '#';


        /*** Cache ***/

        public static Dictionary<Type, char> Cache = new Dictionary<Type, char>();

        public static char GetTexture(Type type)
        {
            if (Cache.ContainsKey(type))
                return Cache[type];

            var value = (char)typeof(Assets).GetField(type.Name).GetValue(null);

            Cache[type] = value;

            return value;
        }
    }
}
