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
            SavedOverlay,
	        Overlay,
            Screen
	    }

        public enum MenuDesigion
        {
            Play,
            Exit
        }

        public Point ExaminatedPoint = null;
        public List<Point> BlastedPoints = new List<Point>();
        private char[,] OverlaySaver;
        private char[,] Overlay;
        private char[,] Screen;
        private string Buffer;

        private const char Fog = Assets.Fog;
        private const char PlayerVisionMarker = Assets.PlayerVisionMarker;
        private const char ExaminatedMarker = Assets.ExaminatedMarker;
        private const char PlayerVision = Assets.PlayerVision;
        private const char BlastMarker = Assets.Blast;

        public Display()
        {
            Overlay = new char[SizeConstants.GameScreenHeight, SizeConstants.GameScreenWidth];
            Screen = new char[SizeConstants.GameScreenHeight, SizeConstants.GameScreenWidth];
        }

        #region Helpers

        public void Put(char c, int x, int y, Destination destination)
        {
            switch (destination)
            {
                case Destination.SavedOverlay:
                    OverlaySaver[y, x] = c;
                    break;
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

        public void Put(string s, int x, int y, Destination destination)
        {
            for (int i = 0; i < s.Length; i++)
            {
                switch (destination)
                {
                    case Destination.SavedOverlay:
                        OverlaySaver[y, x] = s[i];
                        break;
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

        public void ResetBuffer()
        {
            Buffer = null;
        }

        protected void MakeScreen()
        {
            foreach (var entity in GameState.Current.CurrentLevel.Tiles)
            {
                Put(entity.Texture, entity.X, entity.Y, Destination.Screen);
            }

            foreach (var entity in GameState.Current.CurrentLevel.Entities)
            {
                Put(entity.Texture, entity.X, entity.Y, Destination.Screen);
            }
        }

        #endregion

        #region Overlay

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
            for (int x = 0; x < SizeConstants.FieldWidth; x++)
            {
                for (int y = 0; y < SizeConstants.FieldHeight; y++)
                {
                    Put(Fog, x, y, Destination.Overlay);
                }
            }
        }

        private void FillOverlay()
        {
            var ui = UiManager.Current.Render();

            for (int x = SizeConstants.UiMarginWidth; x < SizeConstants.GameScreenWidth; x++)
            {
                for (int y = SizeConstants.UiMarginHeight; y < SizeConstants.GameScreenHeight; y++)
                {
                    Put(ui[y, x - SizeConstants.UiMarginWidth], x, y, Destination.Overlay);
                }
            }
        }

        private void MakeExaminated()
        {
            if (((object)ExaminatedPoint) == null)
                return;

            var tragetX = ExaminatedPoint.X;
            var targetY = ExaminatedPoint.Y;
            // render nearby
            for (int x = Math.Max(tragetX - 1, 0);
                x < Math.Min(tragetX + 2, SizeConstants.FieldWidth); x++)
            {
                for (int y = Math.Max(targetY - 1, 0);
                y < Math.Min(targetY + 2, SizeConstants.FieldHeight); y++)
                {
                    Put(ExaminatedMarker, x, y, Destination.Overlay);
                }
            }
            Put(PlayerVisionMarker, tragetX, targetY, Destination.Overlay);
        }

        private void MakeBlasted()
        {
            foreach (var point in BlastedPoints)
            {
                if (Overlay[point.Y, point.X] == PlayerVisionMarker)
                    Put(BlastMarker, point.X, point.Y, Destination.Overlay);
            }
        }

        private void MakePopup()
        {
            char[,] popup = null;

            if (GameState.Current.PopupOpened)
            {
                popup = UiManager.Current.RenderPopup(GameState.Current.PopupMessage);
            }
            else if (GameState.Current.InventoryOpened)
            {
                popup = UiManager.Current.RenderInventory();
            }
            else if (GameState.Current.ShopOpened)
            {
                popup = UiManager.Current.RenderShop();
            }

            if (popup != null)
            {
                for (int x = 1; x < SizeConstants.InventoryWidth + 1; x++)
                {
                    for (int y = 1; y < SizeConstants.InventoryHeight + 1; y++)
                    {
                        Put(popup[y - 1, x - 1], x, y, Destination.Overlay);
                    }
                }
            }
        }

        private void MakeVision()
        {
            var playerX = GameState.Current.Player.X;
            var playerY = GameState.Current.Player.Y;
            // render nearby
            for (int x = Math.Max(playerX - 1, 0);
                x < Math.Min(playerX + 2, SizeConstants.FieldWidth); x++)
            {
                for (int y = Math.Max(playerY - 1, 0);
                y < Math.Min(playerY + 2, SizeConstants.FieldHeight); y++)
                {
                    Put(PlayerVisionMarker, x, y, Destination.Overlay);
                }
            }
            //render direct
            var dX = playerX;
            var dY = playerY;
            while (!(GameState.Current.CurrentLevel.GetTileAt(dX - 1, dY) is Wall) && dX <= playerX + 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dX++;
            }
            dX = playerX;
            while (!(GameState.Current.CurrentLevel.GetTileAt(dX + 1, dY) is Wall) && dX >= playerX - 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dX--;
            }
            dX = playerX;
            dY = playerY;
            while (!(GameState.Current.CurrentLevel.GetTileAt(dX, dY - 1) is Wall) && dY <= playerY + 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dY++;
            }
            dY = playerY;
            while (!(GameState.Current.CurrentLevel.GetTileAt(dX, dY + 1) is Wall) && dY >= playerY - 3)
            {
                Put(PlayerVisionMarker, dX, dY, Destination.Overlay);
                dY--;
            }
            //render angles
            var topleftfree = !(GameState.Current.CurrentLevel.GetTileAt(playerX - 1, playerY - 1) is Wall);
            var toprightfree = !(GameState.Current.CurrentLevel.GetTileAt(playerX + 1, playerY - 1) is Wall);
            var bottomleftfree = !(GameState.Current.CurrentLevel.GetTileAt(playerX - 1, playerY + 1) is Wall);
            var bottomrightfree = !(GameState.Current.CurrentLevel.GetTileAt(playerX + 1, playerY + 1) is Wall);
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

        protected void MakeOverlay()
        {
            FillOverlay();
            MakeVision();
            MakeExaminated();
            MakeBlasted();
            MakePopup();
        }

        #endregion

        #region Menu & Start/End Screens

        public void ShowStartScreen()
        {
            var desigionMaked = false;
            var desigion = MenuDesigion.Play;
            do
            {
                DisplayManager.Current.DrawMenu(desigion);
                var input = Console.ReadKey(true).Key;

                switch (input)
                {
                    case ConsoleKey.Q:
                        desigionMaked = true;
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.S:
                        if (desigion == MenuDesigion.Play)
                            desigion = MenuDesigion.Exit;
                        else
                            desigion = MenuDesigion.Play;
                        break;
                    default:
                        break;
                }
            } while (!desigionMaked);

            if (desigion == MenuDesigion.Exit)
                Environment.Exit(0);
            else
                GameState.Reset();
        }

        public void DrawMenu(MenuDesigion option)
        {
            Console.Clear();

            var rendered = UiManager.Current.RenderMenu(option);

            foreach (var item in rendered)
            {
                Console.Out.WriteLine(item);
            }
        }

        public void ShowEndScreen()
        {
            var endScreen = new char[SizeConstants.GameScreenHeight, SizeConstants.GameScreenWidth];

            for (int x = 0; x < SizeConstants.GameScreenWidth; x++)
            {
                for (int y = 0; y < SizeConstants.GameScreenHeight; y++)
                {
                    Put(' ', x, y, Destination.Overlay);
                }
            }

            Put("YOU DIED", 15, 20, Destination.Overlay);
            
            Console.Out.Write(Render(false));
            
            MusicManager.Current.Play(Music.Theme.Death);
        }

        #endregion

        #region Drawing

        public string Render(bool makeScreens = true)
        {
            var result = new StringBuilder();

            if (makeScreens)
            {
                MakeScreen();
                MakeOverlay();
            }

            for (int y = 0; y < SizeConstants.GameScreenHeight; y++)
            {
                for (int x = 0; x < SizeConstants.GameScreenWidth; x++)
                {
                    var valueToDisplay = (Overlay[y, x] != PlayerVisionMarker) ? Overlay[y, x] : Screen[y, x];
                    result.Append((valueToDisplay == PlayerVisionMarker) ? PlayerVision : valueToDisplay);
                }
            }

            return result.ToString();
        }

        public void Draw(bool redrawActions = true)
        {
            for (int y = 0; y < SizeConstants.GameScreenHeight; y++)
            {
                for (int x = 0; x < SizeConstants.GameScreenWidth; x++)
                {
                    Overlay[y,x] = (Overlay[y, x] == BlastMarker) ? PlayerVisionMarker : Overlay[y, x];
                }
            }

            var newScreen = DisplayManager.Current.Render();
            var length = newScreen.Length;

            for (int i = 0; i < length; i++)
            {
                if (Buffer == null || newScreen[i] != Buffer[i])
                {
                    var y = i / SizeConstants.GameScreenWidth;
                    var x = i - (y * SizeConstants.GameScreenWidth);
                    var customColored = ResolveColoring(x, newScreen[i]);
                    Console.SetCursorPosition(x, y);

                    Console.Out.Write(newScreen[i]);

                    if (customColored)
                        Console.ResetColor();
                }
            }

            if (redrawActions)
                DrawActionsLine();

            Console.SetCursorPosition(0, SizeConstants.GameScreenHeight + 1);
            Buffer = newScreen;
            ExaminatedPoint = null;
        }

        private void DrawActionsLine()
        {
            for (int i = 0; i <= SizeConstants.GameScreenWidth; i++)
            {
                Console.SetCursorPosition(i, SizeConstants.GameScreenHeight);
                Console.Out.Write(" ");
            }

            Console.SetCursorPosition(0, SizeConstants.GameScreenHeight);
            Console.Out.Write(UiManager.Current.MakeActionsLine());
        }

        private bool ResolveColoring(int x, char newChar)
        {
            var customColored = false;

            if (!GameState.Current.InventoryOpened && !GameState.Current.ShopOpened && !GameState.Current.PopupOpened && x <= SizeConstants.FieldWidth)
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
                else if (newChar == Assets.Item || newChar == Assets.GoldDrop || newChar == Assets.ScrollDrop)
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
                else if (newChar == Assets.Blast)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    customColored = true;
                }
            }

            return customColored;
        }

        #endregion
    }
}
