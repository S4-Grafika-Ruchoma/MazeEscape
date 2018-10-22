using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Sounds
{
    // Klasa obsługująca efekty dzwiękowe
    public class SoundEffects
    {
        public SoundEffect sound { get; private set; } // Dzwiek do obsługi

        public bool Wait { get; internal set; }

        ContentManager Content;

        // Dodawanie nowego dzwięku
        public SoundEffects(SoundEffect sound)
        {
            this.sound = sound;
        }
        

        // Odtwarzanie dzwięku
        public void Play()
        {
            if (!Wait)
            {
                sound.Play();
            }
        }
    }
}