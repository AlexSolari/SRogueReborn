using SRogue.Core;
using SRogue.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager.Current.GenerateWorld();
            //MusicManager.Current.Play();
            do
            {
                Console.Clear();

                Console.Out.Write(DisplayManager.Current.Render());
                Console.Out.Write(UiManager.Current.MakeActionsLine());

                GameManager.Current.GameTick();
                Console.SetWindowSize(80, 27);
                GameManager.Current.ProcessInput(Console.ReadKey().KeyChar);
            } while (true);
        }
    }
}
