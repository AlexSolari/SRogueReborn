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
            Console.WindowHeight += 1; 
            GameManager.Instance.GenerateWorld();
            do
            {
                Console.Clear();
                
                Console.Out.Write(DisplayManager.Instance.Render());

                GameManager.Instance.ProcessInput(Console.ReadKey().KeyChar);
                GameManager.Instance.GameTick();

            } while (true);
            
        }
    }
}
