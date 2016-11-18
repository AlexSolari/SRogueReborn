using SRogue.Core.Common;
using SRogue.Core.Common.Items.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class UI
    {
        public const int UiWidth = 20;
        public const int UiHeight = 25;
        public const int InventoryWidth = 56;
        public const int InventoryHeight = 23;
        public const int MarginWidth = 59;
        public const int MarginHeight = 0;

        public const char UiBorder = Assets.UiBorder;

        public StringBuilder Actions { get; private set; }

        public UI()
        {
            Actions = new StringBuilder();
        }

        public char[,] Render()
        {
            var ui = new char[UiHeight, UiWidth];

            MakeUiBorders(ui);
            MakeDepthmeter(ui);
            MakeHealthmeter(ui);
            MakeStats(ui);
            MakeGold(ui);
            MakeBuffs(ui);

            MakeHints(ui);

            return ui;
        }

        public char[,] RenderInventory()
        {
            var ui = new char[InventoryHeight, InventoryWidth];

            FillPopup(ui);
            RenderPopupBorders(ui);
            MakeInventoryHeader(ui);
            MakeInvenotyItems(ui);

            return ui;
        }

        public char[,] RenderShop()
        {
            var ui = new char[InventoryHeight, InventoryWidth];

            FillPopup(ui);
            MakeShopHeader(ui);
            MakeShopItems(ui);
            RenderPopupBorders(ui);

            return ui;
        }

        public char[,] RenderPopup(string msg)
        {
            var ui = new char[InventoryHeight, InventoryWidth];

            FillPopup(ui);
            AddPopupText(ui, msg);
            RenderPopupBorders(ui);

            return ui;
        }

        #region Inventory

        protected void MakeInvenotyItems(char[,] ui)
        {
            var inv = GameState.Current.Inventory;
            var index = 3;
            if (inv.Weapon.Item != null)
            {
                Put(Padding("Equiped: {0}".FormatWith(inv.Weapon.Item.Name), InventoryWidth), 1, index++, ui);
            }
            if (inv.Head.Item != null)
            {
                Put(Padding("Equiped: {0}".FormatWith(inv.Head.Item.Name), InventoryWidth), 1, index++, ui);
            }
            if (inv.Chest.Item != null)
            {
                Put(Padding("Equiped: {0}".FormatWith(inv.Chest.Item.Name), InventoryWidth), 1, index++, ui);
            }
            if (inv.Legs.Item != null)
            {
                Put(Padding("Equiped: {0}".FormatWith(inv.Legs.Item.Name), InventoryWidth), 1, index++, ui);
            }
            if (inv.Foot.Item != null)
            {
                Put(Padding("Equiped: {0}".FormatWith(inv.Foot.Item.Name), InventoryWidth), 1, index++, ui);
            }

            var backpack = inv.Backpack;
            index++;

            foreach (var item in backpack)
            {
                var formatStr = (item == inv.Selected) ? ">> {0} <<" : "{0}";
                Put(Padding(formatStr.FormatWith((item as ItemBase).Name), InventoryWidth), 1, index++, ui);
            }
        }

        protected void MakeInventoryHeader(char[,] ui)
        {
            Put(Padding("INVENTORY", InventoryWidth), 1, 1, ui);
            Put(Padding("w,s - navigate, q - equip/activate, e - sell", InventoryWidth), 1, 2, ui);
        }

        #endregion

        #region Shop
        protected void MakeShopHeader(char[,] ui)
        {
            Put(Padding("SHOP", InventoryWidth), 1, 1, ui);
            Put(Padding("w,s - navigate, q - activate", InventoryWidth), 1, 2, ui);

            var messagemultiline = GameState.Current.Shop.Message.Split('#');
            for (int i = 0; i < messagemultiline.Length; i++)
            {
                Put(Padding(messagemultiline[i], InventoryWidth), 1, i + 4, ui);
            }
            
        }
        protected void MakeShopItems(char[,] ui)
        {
            var shp = GameState.Current.Shop;
            var index = 15;

            var formatStr = (shp.CurrentOption == State.CityShop.Options.Training) ? ">> {0} <<" : "{0}";
            Put(Padding(formatStr.FormatWith(State.CityShop.Options.Training.FormatWith(Math.Pow(2, GameState.Current.TrainingLevel))), InventoryWidth), 1, index++, ui);
            formatStr = (shp.CurrentOption == State.CityShop.Options.Story) ? ">> {0} <<" : "{0}";
            Put(Padding(formatStr.FormatWith(State.CityShop.Options.Story), InventoryWidth), 1, index++, ui);
            formatStr = (shp.CurrentOption == State.CityShop.Options.Exit) ? ">> {0} <<" : "{0}";
            Put(Padding(formatStr.FormatWith(State.CityShop.Options.Exit), InventoryWidth), 1, index++, ui);
        }
        #endregion

        #region Popup

        protected void AddPopupText(char[,] ui, string text)
        {
            Put(Padding(text, InventoryWidth), 1, 10, ui);
            Put(Padding(">> q - OK <<", InventoryWidth), 1, 15, ui);
        }

        protected void FillPopup(char[,] ui)
        {
            for (int x = 0; x < InventoryWidth; x++)
            {
                for (int y = 0; y < InventoryHeight; y++)
                {
                    ui[y, x] = ' ';
                }
            }
        }

        protected void RenderPopupBorders(char[,] ui)
        {
            var x = 0;
            var y = 0;

            while (x < InventoryWidth - 1)
            {
                Put(UiBorder, x, y, ui);
                x++;
            }
            while (y < InventoryHeight - 1)
            {
                Put(UiBorder, x, y, ui);
                y++;
            }
            while (x > 0)
            {
                Put(UiBorder, x, y, ui);
                x--;
            }
            while (y > 0)
            {
                Put(UiBorder, x, y, ui);
                y--;
            }
        }

        #endregion

        #region Utility

        public void LoseGame()
        {
            Console.Clear();
            DisplayManager.Current.ShowEndScreen();
            Console.ReadKey();
            Environment.Exit(0);
        }

        private void Put(char c, int x, int y, char[,] ui)
        {
            ui[y, x] = c;
        }

        private void Put(string s, int x, int y, char[,] ui, bool padding = false)
        {
            if (padding)
            {
                s = Padding(s);
            }

            for (int i = 0; i < s.Length; i++)
            {
                if (x + i >= 56)
                    break;

                ui[y, x + i] = s[i];
            }
        }

        protected string Padding(string str, int width = UiWidth)
        {
            var result = new StringBuilder(str);

            if (str.Length % 2 != 1)
            {
                result.Insert(0, " ");
            }

            while (result.Length < width - 3)
            {
                result.Insert(0, " ");
                result.Append(" ");
            }

            return result.ToString();
        }

        public string MakeActionsLine()
        {
            var result = string.Join("", Actions.ToString().Take(80));
            Actions.Clear();
            return result;
        }

        #endregion

        #region UI

        protected void MakeUiBorders(char[,] ui)
        {
            int x = 0;
            int y = 0;

            while (x < UiWidth - 1)
            {
                Put(UiBorder, x, y, ui);
                x++;
            }
            while (y < UiHeight - 1)
            {
                Put(UiBorder, x, y, ui);
                y++;
            }
            while (x > 0)
            {
                Put(UiBorder, x, y, ui);
                x--;
            }
            while (y > 0)
            {
                Put(UiBorder, x, y, ui);
                y--;
            }
        }

        protected void MakeDepthmeter(char[,] ui)
        {
            Put("DEPTH: {0}".FormatWith(GameState.Current.Depth), 1, 1, ui, true);
        }

        protected void MakeHealthmeter(char[,] ui)
        {
            Put("HP: {0}/{1}".FormatWith((int)GameState.Current.Player.Health, GameState.Current.Player.HealthMax), 1, 2, ui, true);
        }

        protected void MakeStats(char[,] ui)
        {
            Put("STATS:", 1, 3, ui, true);
            Put("damage: {0}".FormatWith(GameState.Current.Player.SummarizeAttack()), 1, 4, ui, true);
            Put("armor: {0}".FormatWith(GameState.Current.Player.SummarizeArmor()), 1, 5, ui, true);
            Put("resist: {0}".FormatWith(GameState.Current.Player.SummarizeResist()), 1, 6, ui, true);
        }

        protected void MakeGold(char[,] ui)
        {
            Put("GOLD: {0}".FormatWith(GameState.Current.Gold), 1, 7, ui, true);
        }

        protected void MakeBuffs(char[,] ui)
        {
            Put("BUFFS:".FormatWith(GameState.Current.Depth), 1, 8, ui, true);
            var firstfive = GameState.Current.Player.Buffs.Take(5);
            int index = 9;
            foreach (var buff in firstfive)
            {
                Put("{0}".FormatWith(buff.BuffName), 1, index, ui, true);
                index++;
            }
        }

        protected void MakeHints(char[,] ui)
        {
            Put("e - examinate".FormatWith(GameState.Current.Depth), 1, 20, ui, true);
            Put("i - inventory".FormatWith(GameState.Current.Depth), 1, 21, ui, true);
            Put("wasd - control".FormatWith(GameState.Current.Depth), 1, 22, ui, true);
        }
        #endregion
    }
}
