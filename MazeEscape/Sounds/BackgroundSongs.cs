using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Sounds {
    // Klasa obsługująca muzyke tła
    class BackgroundSongs {
        Song song; // Muzyka

        // Dodanie nowej piosenki
        public BackgroundSongs(Song song,  bool IsRepeating, float Volume)
        {
            this.song = song;//song;
            Repeating(IsRepeating);
            ChangeVolume(Volume);
        }

        // Zmiana piosenki
        public void ChangeSong(string SongPath) {
            Stop();
            Play();
        }

        // Czy piosenka ma być powatarzana
        public void Repeating(bool Repeating) {
            MediaPlayer.IsRepeating = Repeating;
        }

        // Głośność piosenki
        public void ChangeVolume(float Volume) {
            MediaPlayer.Volume = Volume;
        }

        public void Play() {
            MediaPlayer.Play(song);
        }
        public void Stop() {
            MediaPlayer.Stop();
        }

        public void Resume() {
            MediaPlayer.Resume();
        }
        public void Pause() {
            MediaPlayer.Pause();
        }
    }
}