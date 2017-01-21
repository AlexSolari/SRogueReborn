using SRogue.Core.Common;
using SRogue.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core
{
    class GameState : Singleton<State>
    {
        public static void Reset()
        {
            _instance = new State();
            Ai.Container.ResetPathMap();
            DisplayManager.Current.ResetOverlay();
            GameManager.Current.GenerateWorld();
        }
    }
}
