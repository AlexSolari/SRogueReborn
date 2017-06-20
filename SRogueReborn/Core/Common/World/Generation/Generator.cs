using SRogue.Core.Entities.Concrete.Entities;
using SRogue.Core.Entities.Concrete.Tiles;
using SRogue.Core.Entities.Interfaces;
using SRogue.Core.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common.World.Generation
{
    public class Generator
    {
        public GameLevel GenerateWorld()
        {
            var result = new GameLevel();

            var isCity = GameState.Current.Depth % 5 == 0 && GameState.Current.Depth <= 35;
            var isBoss = !isCity && GameState.Current.Depth % 5 == 4 || GameState.Current.Depth > 35;

            result.Tiles = new ITile[25, 59];
            result.Entities = new List<IUnit>();

            var roomsCount = Rnd.Current.Next(6, 14);

            if (isCity)
                roomsCount = 3;

            var centers = new List<Point>();
            for (int i = 0; i < roomsCount; i++)
            {
                result.GenerateRoom(centers);
            }

            result.GenerateCorridors(centers);

            result.Fill();

            if (!isCity)
            {

                result.GenerateTraps();

                result.GenerateEnemies(isBoss);

            }
            result.AddPlayer(centers);

            if (isCity)
            {
                GameState.Current.PopupMessage = "This place is safe. There is a shop nearby.";
                GameState.Current.PopupOpened = true;
                result.GenerateCity(centers);
            }
            else
            {
                result.AddGoldPickups();
            }

            if (isBoss)
            {
                GameState.Current.PopupMessage = "You feel evil presense of powerful creature.";
                GameState.Current.PopupOpened = true;
            }

            result.GenerateExit(centers);
            DisplayManager.Current.ResetOverlay();

            DisplayManager.Current.SaveOverlay();

            if (isBoss)
            {
                MusicManager.Current.Play(Music.Theme.Boss);
            }
            else if (isCity)
            {
                MusicManager.Current.Play(Music.Theme.City);
            }
            else
            {
                MusicManager.Current.Play(Music.Theme.Default);
            }

            return result;
        }
    }
}
