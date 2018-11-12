using System;
using Microsoft.Xna.Framework.Media;

namespace MazeEscape.Sounds
{
    // Klasa obsługująca muzyke tła
    public class BackgroundSongs : ISound
    {
        public Song song { get; private set; } // Muzyka

        // Dodanie nowej piosenki
        public BackgroundSongs(Song song, bool IsRepeating, float Volume)
        {
            this.song = song;
            Repeating(IsRepeating);
            ChangeVolume(Volume);
        }

        // Czy piosenka ma być powatarzana
        public void Repeating(bool Repeating)
        {
            MediaPlayer.IsRepeating = Repeating;
        }

        // Głośność piosenki
        public void ChangeVolume(float Volume)
        {
            MediaPlayer.Volume = Volume;
        }

        public void Play(float volume = 0)
        {
            if (AppConfig.PLAY_SOUNDS)
            {
                MediaPlayer.Play(song);
            }
        }
        public void Stop()
        {
            MediaPlayer.Stop();
        }

        public void Resume()
        {
            MediaPlayer.Resume();
        }

        public TimeSpan Duration => song.Duration;

        public void Pause()
        {
            MediaPlayer.Pause();
        }
    }
}