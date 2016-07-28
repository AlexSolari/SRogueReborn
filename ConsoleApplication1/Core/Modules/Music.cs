using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class Music
    {
        public static List<int> Notes = new List<int>();

        private static void AddNote(int freq)
        {
            Notes.Add(freq);
        }

        public void Play()
        {
            AddNote(262);
            AddNote(294);
            AddNote(330);
            AddNote(350);
            AddNote(392);
            AddNote(440);
            AddNote(494);
            AddNote(523);
            AddNote(587);
            AddNote(659);
            AddNote(698);
            AddNote(783);
            AddNote(880);
            AddNote(988); 
            
            var melody = new Thread(Melody);
            melody.Start();
        }

        private static void Melody()
        {
            var generator = new Random();
            while(true)
            {
                var pos = 0;
                switch (generator.Next(4))
                {
                    case 0:
                        pos = generator.Next(4, Notes.Count);
                        Console.Beep(Music.Notes[pos], 333);
                        Console.Beep(Music.Notes[pos-1], 333);
                        Console.Beep(Music.Notes[pos-2], 333);
                        Console.Beep(Music.Notes[pos-3], 333);
                        break;
                    case 1:
                        pos = generator.Next(2, Notes.Count - 4);
                        Console.Beep(Music.Notes[pos], 333);
                        Console.Beep(Music.Notes[pos-2], 333);
                        Console.Beep(Music.Notes[pos], 333);
                        Console.Beep(Music.Notes[pos-2], 333);
                        Console.Beep(Music.Notes[pos], 333);
                        Console.Beep(Music.Notes[pos+2], 333);
                        Console.Beep(Music.Notes[pos], 333);
                        Console.Beep(Music.Notes[pos+2], 333);
                        break;
                    case 2:
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        break;
                    case 3:
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[0], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[0], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[0], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[generator.Next(Notes.Count)], 333);
                        Console.Beep(Music.Notes[0], 333);
                        break;
                    default:
                        break;
                }
                
            }
        }
    }
}
