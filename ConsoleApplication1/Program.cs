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
    internal static class Program
    {
        static Program()
        {
            Console.Title = "SRogue";

			Console.SetWindowSize(80, 27);

			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Unix:
					Console.WriteLine("Please, set the console window size as " +
						"80 x 27 or more and press any key");
					Console.ReadKey(true); 
					break;

				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
					break;

				default:
					Console.WriteLine("Your platform is probably not supported, " +
						"we're sorry if there will be any bugs");
					break;
			}

            AiManager.Current.InitializeDefaults();
        }

        private static ssssssssvoid Main(string[] args)
        {
            GameManager.Current.GenerateWorld();
#if !DEBUG
            MusicManager.Current.Play();
#endif
            do
            {
#if DEBUG
                var startTime = DateTime.Now;
#endif
                DisplayManager.Current.Draw();
#if DEBUG
                var span = DateTime.Now.Ticks - startTime.Ticks;
                    
                using (var writer = new System.IO.StreamWriter("log.txt", true))
                {
					writer.WriteLine($"{span} ticks taken to draw ({span / TimeSpan.TicksPerMillisecond} ms)");
                }
#endif
                GameManager.Current.ProcessInput(Console.ReadKey().Key);
			} while (true);
        }
    }
}
