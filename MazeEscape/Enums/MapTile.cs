using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeEscape.Enums
{
    public enum MapTile
    {
        Wall = -1,
        Empty = 0,
        DeadEnd = 1,
        LadderUp = 2,
        LadderDown = 3,
        StartCell = 4,
        EndCell = 5,
    }
}
