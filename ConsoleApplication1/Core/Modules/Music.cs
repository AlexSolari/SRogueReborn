using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SRogue.Core.Modules
{
    public class Music
    {
        public enum Theme
        {
            Default, 
            Boss,
            City,
            Death,
            None
        }

        public Theme CurrentTheme = Theme.None;

        public SoundPlayer LoopPlayer = new SoundPlayer();

        public void Play(Theme level)
        {
            if (CurrentTheme == level)
                return;

            MusicManager.Current.LoopPlayer.Stop();
            var theme = string.Empty;

            switch (level)
            {
                case Theme.Boss:
                    theme = "res/ost/boss.wav";
                    break;
                case Theme.City:
                    theme = "res/ost/city.wav";
                    break;
                case Theme.Default:
                    theme = "res/ost/level.wav";
                    break;
                case Theme.Death:
                    theme = "res/ost/death.wav";
                    break;
                default:
                    return;
            }

            CurrentTheme = level;
            LoopPlayer.SoundLocation = theme;
            LoopPlayer.PlayLooping();
        }
    }
}
