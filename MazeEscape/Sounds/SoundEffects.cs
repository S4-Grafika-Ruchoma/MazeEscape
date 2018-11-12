using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MazeEscape.Sounds
{
    // Klasa obsługująca efekty dzwiękowe
    public class SoundEffects : ISound
    {
        public SoundEffect sound { get; private set; } // Dzwiek do obsługi

        public bool Wait { get; internal set; }
        
        // Dodawanie nowego dzwięku
        public SoundEffects(SoundEffect sound)
        {
            this.sound = sound;
        }

        // Dodawanie nowego dzwięku
        public SoundEffects(ContentManager Content, string soundPath)
        {
            this.sound = Content.Load<SoundEffect>(soundPath);
        }


        // Odtwarzanie dzwięku
        public void Play(float volume = 0)
        {
            if (!Wait &&  AppConfig.PLAY_SOUNDS)
            {
                sound.Play(volume,0,0);
            }
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
        }

        public void Resume()
        {
            throw new System.NotImplementedException();
        }

        public TimeSpan Duration => sound.Duration;
    }
}