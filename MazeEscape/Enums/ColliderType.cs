using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeEscape.Enums
{
    public enum ColliderType
    {
        Wall,//0
        LadderEnter,
        LadderExit,
        Empty,//3
        Camera,
        Enemy,
        Collectable,
        Floor,
        EnemyStartPosition,
    }
}
