using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeEscape.Enums;
using MazeEscape.GameObjects;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1;

namespace MazeEscape.AI
{
    public class Spot
    {
        public ColliderType Type;
        public Point Position;

        public float f { get; set; }
        public float g { get; set; }
        public float h { get; set; }
        public Spot previous { get; set; }

        public Spot(Point point, ColliderType wall)
        {
            Reset();
            this.Position = point;
            this.Type = wall;
        }

        public List<Spot> GetNeighbors(List<List<Spot>> map)
        {
            var list = new List<Spot>();

            if (Position.X + 1 < map.Count && IsAllowedSpot(map[Position.X + 1][Position.Y]))
            {
                list.Add(map[Position.X + 1][Position.Y]);
            }
            if (Position.X - 1 >= 0 && IsAllowedSpot(map[Position.X - 1][Position.Y]))
            {
                list.Add(map[Position.X - 1][Position.Y]);
            }
            if (Position.Y + 1 < map[Position.X].Count && IsAllowedSpot(map[Position.X][Position.Y + 1]))
            {
                list.Add(map[Position.X][Position.Y + 1]);
            }
            if (Position.Y - 1 >= 0 && IsAllowedSpot(map[Position.X][Position.Y - 1]))
            {
                list.Add(map[Position.X][Position.Y - 1]);
            }

            return list;
        }

        private bool IsAllowedSpot(Spot spot)
        {
            return spot.Type == ColliderType.Empty || spot.Type == ColliderType.Floor ||
                   spot.Type == ColliderType.Collectable;
        }

        public void Reset()
        {
            previous = null;
            f = 0;
            h = 0;
            g = 0;
        }
    }

    public class Pathfinding
    {
        private List<List<Spot>> GameMap;
        public List<Vector2> PositionList;

        public Pathfinding(List<List<Spot>> gameMap, Enemy enemy)
        {
            GameMap = gameMap;

            var enemySpot = new Spot(new Point((int)enemy.Position.X / 2, (int)enemy.Position.Z / 2), ColliderType.Empty)
            {
                f = 0,
                g = 0,
                h = 0,
            };

            CreatePath(enemySpot);
        }

        // Tworzenie ścieżki miedzy portalami i znajdzkami
        public void CreatePath(Spot enemyPos)
        {
            PositionList = new List<Vector2>();
            var collectableList = GameMap.SelectMany(a => a).Where(a => a.Type == ColliderType.Collectable).ToList();

            var list = FindPath(enemyPos, collectableList[0]).Select(a => new Vector2(a.Position.X, a.Position.Y)).ToList();
            list.Reverse();
            PositionList.AddRange(list);
            collectableList[0].Reset();

            for (int i = 1; i < collectableList.Count; i++)
            {
                list = FindPath(collectableList[i - 1], collectableList[i]).Select(a => new Vector2(a.Position.X, a.Position.Y)).ToList();
                list.Reverse();
                PositionList.AddRange(list);
                collectableList[i - 1].Reset();
                collectableList[i].Reset();
            }

            list = FindPath(collectableList[collectableList.Count - 1], collectableList[0]).Select(a => new Vector2(a.Position.X, a.Position.Y)).ToList();
            list.Reverse();
            PositionList.AddRange(list);
            collectableList[collectableList.Count - 1].Reset();
        }

        private List<Spot> closedSet;
        private List<Spot> openSet;

        private List<Spot> FindPath(Spot start, Spot stop)
        {
            var moves = new List<Spot>();

            closedSet = new List<Spot>();
            openSet = new List<Spot>()
            {
                start
            };


            while (openSet.Any())
            {
                var winner = 0;
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].f < openSet[winner].f)
                    {
                        winner = i;
                    }
                }

                var current = openSet[winner];

                if (current.Position == stop.Position)
                {
                    var tmpCurrent = current;
                    moves.Add(tmpCurrent);

                    while (tmpCurrent.previous != null)
                    {
                        moves.Add(tmpCurrent.previous);
                        tmpCurrent = tmpCurrent.previous;
                    }

                    break;
                }

                openSet.Remove(current);
                closedSet.Add(current);

                var neighbors = current.GetNeighbors(GameMap);

                for (int i = 0; i < neighbors.Count; i++)
                {
                    var neighbor = neighbors[i];

                    if (!closedSet.Contains(neighbor))
                    {
                        var tempG = current.g + 1;

                        if (openSet.Contains(neighbor))
                        {
                            if (tempG < neighbor.g)
                            {
                                neighbor.g = tempG;
                            }
                        }
                        else
                        {
                            neighbor.g = tempG;
                            openSet.Add(neighbor);
                        }

                        neighbor.h = Heuristic(neighbor, stop);
                        neighbor.f = neighbor.g + neighbor.h;
                        neighbor.previous = current;
                    }
                }
            }


            return moves;
        }

        private float Heuristic(Spot neighbor, Spot stop)
        {
            return (float)Math.Sqrt(Math.Pow(stop.Position.X - neighbor.Position.X, 2) + Math.Pow(stop.Position.Y - neighbor.Position.Y, 2));
        }
    }
}
