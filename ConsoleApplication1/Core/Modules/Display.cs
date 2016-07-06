using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class Display
    {
        public enum Destination
    	{
	        Overlay,
            Screen
	    }

        public readonly int Width = 80;
        public readonly int Height = 25;
        private char[,] Overlay;
        private char[,] Screen;

        public Display()
        {
            Overlay = new char[Height, Width];
            Screen = new char[Height, Width];
        }

        protected void Put(char c, int x, int y, Destination destination)
        {
            switch (destination)
            {
                case Destination.Overlay:
                    Overlay[y, x] = c;
                    break;
                case Destination.Screen:
                    Screen[y, x] = c;
                    break;
                default:
                    break;
            }
        }

        protected void Put(string s, int x, int y, Destination destination)
        {
            for (int i = 0; i < s.Length; i++)
            {
                switch (destination)
                {
                    case Destination.Overlay:
                        Overlay[y, x + i] = s[i];
                        break;
                    case Destination.Screen:
                        Screen[y, x + i] = s[i];
                        break;
                    default:
                        break;
                }
            }
        }

        protected void MakeScreen()
        {
            foreach (var entity in GameManager.Instance.Tiles)
            {
                Screen[entity.Y, entity.X] = entity.Texture;
            } 

            foreach (var entity in GameManager.Instance.Entities)
            {
                Screen[entity.Y, entity.X] = entity.Texture;
            }
        }

        public string Render()
        {
            var result = new StringBuilder();

            MakeScreen();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                     result.Append((char.IsLetterOrDigit(Overlay[y, x])) ? Overlay[y, x] : Screen[y, x]);
                }
            }

            return result.ToString();
        }
    }
}
