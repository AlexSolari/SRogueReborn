using SRogue.Core;
using SRogue.Core.Entities;
using SRogue.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue
{
    class Program
    {
        static void Init()
        {
            Console.Title = "SRogue";
            Console.SetWindowSize(80, 27);

            AiManager.Current.RegisterAllFrom<Ai.Container>();
        }

        static void Main(string[] args)
        {
            Init();

            GameManager.Current.GenerateWorld();
#if !DEBUG
            MusicManager.Current.Play();
#endif
            do
            {
                //Console.Clear();
                DisplayManager.Current.Draw();
                GameManager.Current.ProcessInput(Console.ReadKey().KeyChar);
            } while (true);
        }
    }
}
