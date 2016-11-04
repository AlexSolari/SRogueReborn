using SRogue.Core.Common;
using SRogue.Core.Entities.Concrete.Tiles;
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

        public Point ExaminatedPoint = null;
        public readonly int Width = 79;
        public readonly int Height = 25;
        public readonly int FieldWidth = 59;
        public readonly int FieldHeight = 25;
        private char[,] OverlaySaver;
        private char[,] Overlay;
        private char[,] Screen;
        private string Buffer;

        private const char Fog = Assets.Fog;
        private const char PlayerVisionMarker = Assets.PlayerVisionMarker;
        private const char ExaminatedMarker = Assets.ExaminatedMarker;
        private const char PlayerVision = Assets.PlayerVision;

        public Display()
        {
            Overlay = new char[Height, Width];
            Screen = new char[Height, Width];
        }

        public void SaveOverlay()
        {
            OverlaySaver = (char[,])Overlay.Clone();
        }

        public void LoadOverlay()
        {
            Overlay = (char[,])OverlaySaver.Clone();
        }


        public void ResetOverlay()
        {
            for (int x = 0; x < FieldWidth; x++)
            {
                for (int y = 0; y < FieldHeight; y++)
                {
                    Put(Fog, x, y, Destination.Overlay);
                }
            }
        }

        public void Put(char c, int x, int y, Destination destination)
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

        public void ShowEndScreen()
        {
            var endScreen = new char[Height, Width];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Put(' ', x, y, Destination.Overlay);
                }
            }

            Put("YOU DIED", 15, 20, Destination.Overlay);

            Console.Out.Write(Render(false));

            MusicManager.Current.Play(Music.Theme.Death);
        }

        public void Put(string s, int x, int y, Destination destination)
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

        protected void MakeOverlay()
        {
            FillOverlay();
            MakeVision();
            MakeExaminated();
            MakePopup();
        }

        private void MakeExaminated()
        {
            if (((object)ExaminatedPoint) == null)
                return;

            var tragetX = ExaminatedPoint.X;
            var targetY = ExaminatedPoint.Y;
            // render nearby
            for (int x = Math.Max(tragetX - 1, 0);
                x < Math.Min(tragetX + 2, FieldWidth); x++)
            {
                for (int y = Math.Max(targetY - 1, 0);
                y < Math.Min(targetY + 2, FieldHeight); y++)
                {
                    Put(ExaminatedMarker, x, y, Destination.Overlay);
                }
            }
            Put(PlayerVisionMarker, tragetX, targetY, Destination.Overlay);
        }

        private void FillOverlay()
        {
            var ui = UiManager.Current.Render();

            for (int x = UI.MarginWidth; x < Width; x++)
            {
                for (int y = UI.MarginHeight; y < Height; y++)
                {
                    Put(ui[y, x - UI.MarginWidth], x, y, Destination.Overlay);
                }
            }
        }

        private void MakePopup()
        {
            char[,] popup = null;

            if (GameManager.Current.PopupOpened)
            {
                popup = UiManager.Current.RenderPopup(GameManager.Current.PopupMessage);
            }
            else if (GameManager.Current.InventoryOpened)
            {
                popup = UiManager.Current.RenderInventory();
            }
            else if (GameManager.Current.ShopOpened)
            {
                popup = UiManager.Current.RenderShop();
            }

            if (popup != null)
            {
                for (int x = 1; x < UI.InventoryWidth + 1; x++)
                {
                    for (int y = 1; y < UI.InventoryHeight + 1; y++)
                    {
                        Put(popup[y - 1, x - 1], x, y, Destination.Overlay);
                    }
                }
            }
        }

        private void MakeVision()
        {
            var playerX = GameManager.Current.Player.X;
            var playerY = GameManager.Current.Player.Y;
            // render nearby
            for (int x = Math.Max(playerX - 1, 0);
                x < Math.Min(playerX + 2, FieldWidth); x++)
            {
                for (int y = Math.Max(playerY - 1, 0);
                y < Math.Min(playerY + 2, FieldHeight); y++)
                {
                    Put(PlayerVisionMarker, x, y, Destination.Overlay);
                }
            }
            //render direct
            var dX = playerX;
            var dY = playerY;
            while (!GameManager.Current.GetTilesAt(dX - 1, dY).Any(t => t is Wall) && dX <= playerX + 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dX++;
            }
            dX = playerX;
            while (!GameManager.Current.GetTilesAt(dX + 1, dY).Any(t => t is Wall) && dX >= playerX - 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dX--;
            }
            dX = playerX;
            dY = playerY;
            while (!GameManager.Current.GetTilesAt(dX, dY - 1).Any(t => t is Wall) && dY <= playerY + 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dY++;
            }
            dY = playerY;
            while (!GameManager.Current.GetTilesAt(dX, dY + 1).Any(t => t is Wall) && dY >= playerY - 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dY--;
            }
            //render angles
            var topleftfree = !GameManager.Current.GetTilesAt(playerX - 1, playerY - 1).Any(t => t is Wall);
            var toprightfree = !GameManager.Current.GetTilesAt(playerX + 1, playerY - 1).Any(t => t is Wall);
            var bottomleftfree = !GameManager.Current.GetTilesAt(playerX - 1, playerY + 1).Any(t => t is Wall);
            var bottomrightfree = !GameManager.Current.GetTilesAt(playerX + 1, playerY + 1).Any(t => t is Wall);
            if (topleftfree)
            {
                Put(PlayerVisionMarker, playerX - 2, playerY - 1, Destination.Overlay);
                Put(PlayerVisionMarker, playerX - 1, playerY - 2, Destination.Overlay);
            }
            if (toprightfree)
            {
                Put(PlayerVisionMarker, playerX + 2, playerY - 1, Destination.Overlay);
                Put(PlayerVisionMarker, playerX + 1, playerY - 2, Destination.Overlay);
            }
            if (bottomleftfree)
            {
                Put(PlayerVisionMarker, playerX - 2, playerY + 1, Destination.Overlay);
                Put(PlayerVisionMarker, playerX - 1, playerY + 2, Destination.Overlay);
            }
            if (bottomrightfree)
            {
                Put(PlayerVisionMarker, playerX + 2, playerY + 1, Destination.Overlay);
                Put(PlayerVisionMarker, playerX + 1, playerY + 2, Destination.Overlay);
            }
        }

        protected void MakeScreen()
        {
            foreach (var entity in GameManager.Current.Tiles)
            {
                Put(entity.Texture, entity.X, entity.Y, Destination.Screen);
            }

            foreach (var entity in GameManager.Current.Entities)
            {
                Put(entity.Texture, entity.X, entity.Y, Destination.Screen);
            }
        }
        
        public string Render(bool makeScreens = true)
        {
            var result = new StringBuilder();

            if (makeScreens)
            {
                MakeScreen();
                MakeOverlay();
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var valueToDisplay = (Overlay[y, x] != PlayerVisionMarker) ? Overlay[y, x] : Screen[y, x];
                    result.Append((valueToDisplay == PlayerVisionMarker) ? PlayerVision : valueToDisplay);
                }
            }

            return result.ToString();
        }

        public void Draw(bool redrawActions = true)
        {
            var newScreen = DisplayManager.Current.Render();
            var length = newScreen.Length;

            for (int i = 0; i < length; i++)
            {
                if (Buffer == null || newScreen[i] != Buffer[i])
                {
                    var y = i / Width;
                    var x = i - (y * Width);
                    var customColored = ResolveColoring(x, newScreen[i]);
                    Console.SetCursorPosition(x, y);

                    Console.Out.Write(newScreen[i]);

                    if (customColored)
                        Console.ResetColor();
                }
            }

            if (redrawActions)
                DrawActionsLine();

            Console.SetCursorPosition(0, Height + 1);
            Buffer = newScreen;
            ExaminatedPoint = null;
        }

        private void DrawActionsLine()
        {
            for (int i = 0; i <= Width; i++)
            {
                Console.SetCursorPosition(i, Height);
                Console.Out.Write(" ");
            }

            Console.SetCursorPosition(0, Height);
            Console.Out.Write(UiManager.Current.MakeActionsLine());
        }

        private bool ResolveColoring(int x, char newChar)
        {
            var customColored = false;

            if (!GameManager.Current.InventoryOpened && !GameManager.Current.ShopOpened && !GameManager.Current.PopupOpened && x <= FieldWidth)
            {
                if (newChar == Assets.Zombie)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    customColored = true;
                }
                else if (newChar == Assets.Floor || newChar == Assets.Wall)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    customColored = true;
                }
                else if (newChar == Assets.Ghost)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    customColored = true;
                }
                else if (newChar == Assets.ZombieBoss)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    customColored = true;
                }
                else if (newChar == Assets.Item)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    customColored = true;
                }
                else if (newChar == Assets.Exit)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    customColored = true;
                }
                else if (newChar == Assets.ItemShop)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    customColored = true;
                }
                else if (newChar == Assets.ExaminatedMarker)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    customColored = true;
                }
            }

            return customColored;
        }
    }
}
