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

            AiManager.Current.InitializeDefaults();
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
#if DEBUG
                var startTime = DateTime.Now;
#endif

                DisplayManager.Current.Draw();

#if DEBUG
                var span = DateTime.Now.Ticks - startTime.Ticks;
                    
                using (var writer = new System.IO.StreamWriter("log.txt", true))
                {
                    writer.WriteLine("{0} ticks taken to draw ({1} ms)".FormatWith(span, span / TimeSpan.TicksPerMillisecond));
                }
#endif
                GameManager.Current.ProcessInput(Console.ReadKey().KeyChar);
            } while (true);
        }
    }
}
