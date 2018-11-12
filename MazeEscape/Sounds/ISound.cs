using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeEscape.Sounds
{
    public interface ISound
    {
        void Play(float volume = 0);
        void Pause();
        void Stop();
        void Resume();

        TimeSpan Duration { get; }
    }
}
