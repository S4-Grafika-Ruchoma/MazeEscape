using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace MazeEscape.MazeGen
{
    class RectCell
    {
        private bool northWall;
        private bool southWall;
        private bool westWall;
        private bool eastWall;
        private bool isDeadEnd;
        private bool hasLadderUp;
        private bool hasLaddetDown;
        private bool errorFlag;
        private Point point;
        private int _nVisited;
        private bool visited;

        public RectCell(Point Point)
        {
            this.point = Point;
            northWall = true;
            southWall = true;
            westWall = true;
            eastWall = true;

            errorFlag = false;
            visited = false;


            isDeadEnd = false;
            hasLadderUp = false;
            hasLaddetDown = false;
            _nVisited = 0;
        }
        public enum Walls
        {
            Invalid,
            North,
            South,
            East,
            West,
        }

        public bool NorthWall { get => northWall; }
        public bool SouthWall { get => southWall; }
        public bool WestWall { get => westWall; }
        public bool EastWall { get => eastWall; }
        public bool IsDeadEnd { get => isDeadEnd; }
        public bool HasLadderUp { get => hasLadderUp; }
        public bool HasLadderDown { get => hasLaddetDown; }
        public Point Point { get => point; }
        public int nVisited { get => _nVisited; }
        public bool ErrorFlag { get => errorFlag; }
        public bool Visited { get => visited; }

        public void RemoveWall(Walls wall)
        {
            switch (wall)
            {
                case Walls.North:
                    {
                        northWall = false;
                        break;
                    }
                case Walls.South:
                    {
                        southWall = false;
                        break;
                    }
                case Walls.East:
                    {
                        eastWall = false;
                        break;
                    }
                case Walls.West:
                    {
                        westWall = false;
                        break;
                    }
                default:
                    {
                        errorFlag = true;
                        break;
                    }
            }
        }

        public void UnsetDeadEnd()
        {
            this.isDeadEnd = false;
        }

        public void Visit()
        {
            _nVisited++;
            visited = true;
        }
        public void setDeadEnd()
        {
            isDeadEnd = true;
        }
        public void setLedderUp()
        {
            hasLadderUp = true;
        }
        public void setLedderDown()
        {
            hasLaddetDown = true;
        }
    }
}
