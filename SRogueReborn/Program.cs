using SRogue.Core;
using SRogue.Core.Entities;
using SRogue.Core.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue
{
    internal static class Program
    {
        static Program()
        {
            Console.Title = "SRogue";

            Console.SetWindowSize(DisplayManager.Current.ScreenWidth, DisplayManager.Current.ScreenHeight);

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    break;

                default:
                    Console.WriteLine("Your platform is probably not supported, we're sorry if there will be any bugs.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey(true);
                    break;
            }

            AiManager.Current.InitializeDefaults();
        }

        private static void Main(string[] args)
        {
            var redrawActionLine = true;
            GameManager.Current.GenerateWorld();

            do
            {
                DisplayManager.Current.Draw(redrawActionLine);
                redrawActionLine = GameManager.Current.ProcessInput(Console.ReadKey(true).Key);
            } while (true);
        }
    }
}
