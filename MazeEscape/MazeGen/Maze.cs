using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeEscape.Enums;
using Microsoft.Xna.Framework;

namespace MazeEscape.MazeGen
{
    class Maze
    {
        private RectCell[,] _Board;
        private int _Height;
        private int _Width;
        private Random _rng;
        private Point _startPoint;
        private Point _currentPoint;
        private Point _endPoint;
        private int _iterations;
        private List<Point> path;

        public RectCell[,] Board { get => _Board; }
        public int Height { get => _Height; }
        public int Width { get => _Width; }
        public int Iterations { get => _iterations; }

        /// <summary>
        /// Maze init with random seed
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public Maze(int width, int height)
        {
            if (height <= 1 || width <= 1)
                throw new ArgumentOutOfRangeException("dimensions", "must be at least 2x2");

            this._Height = height;
            this._Width = width;
            _rng = new Random();
            _startPoint = new Point(_rng.Next() % _Width, _rng.Next() % _Height);
            this._Board = new RectCell[_Width, _Height];
            this.path = new List<Point>();
        }

        public Maze(int width, int height, Random rnd)
        {
            if (height <= 1 || width <= 1)
                throw new ArgumentOutOfRangeException("dimensions", "must be at least 2x2");

            this._Height = height;
            this._Width = width;
            this._rng = rnd;
            _startPoint = new Point(_rng.Next() % _Width, _rng.Next() % _Height);
            this._Board = new RectCell[_Width, _Height];
            this.path = new List<Point>();
        }

        public Maze(int width, int height, int seed)
        {
            if (height <= 1 || width <= 1)
                throw new ArgumentOutOfRangeException("dimensions", "must be at least 2x2");

            this._Height = height;
            this._Width = width;
            this._rng = new Random(seed);
            _startPoint = new Point(_rng.Next()%_Width, _rng.Next()%_Height);
            this._Board = new RectCell[_Width, _Height];
            this.path = new List<Point>();
        }

        public Maze(int width, int height, Random rnd, Point StartingPoint)
        {
            if (height <= 1 || width <= 1)
                throw new ArgumentOutOfRangeException("dimensions", "must be at least 2x2");
            if ((StartingPoint.X < 0 || StartingPoint.X >= width) || (StartingPoint.Y < 0 || StartingPoint.Y >= height))
                throw new ArgumentOutOfRangeException("Starting point", "must be in dimensions");

            this._Height = height;
            this._Width = width;
            this._rng = rnd;
            this._startPoint = StartingPoint;
            this._Board = new RectCell[_Width, _Height];
            this.path = new List<Point>();
        }

        public Maze(int width, int height, int seed, Point StartingPoint)
        {
            if (height <= 1 || width <= 1)
                throw new ArgumentOutOfRangeException("dimensions", "must be at least 2x2");
            if ((StartingPoint.X < 0 || StartingPoint.X >= width) || (StartingPoint.Y < 0 || StartingPoint.Y >= height))
                throw new ArgumentOutOfRangeException("Starting point", "must be in dimensions");

            this._Height = height;
            this._Width = width;
            this._rng = new Random(seed);
            this._startPoint = StartingPoint;
            this._Board = new RectCell[_Width, _Height];
            this.path = new List<Point>();
        }

        /// <summary>
        /// Initialize the board
        /// </summary>
        public void Initialize()
        {
            for (int y = 0; y < _Height; y++)
                for (int x = 0; x < _Width; x++)
                    _Board[x, y] = new RectCell(new Point(x, y));
        }

        public void Generate()
        {
            // first cell
            path.Add(_startPoint);
            _currentPoint = path[path.Count - 1];

            _Board[_currentPoint.X, _currentPoint.Y].Visit();
            _iterations++;

            bool f = false;

            while (!allVisited())
            {
                List<Directions> validDirections = GetAllDirections();

                ValidateMovement(_currentPoint, validDirections);

                // dead end
                if (validDirections.Count == 0)
                {
                   
                    _Board[_currentPoint.X, _currentPoint.Y].setDeadEnd();

                    if (!f)
                    {
                        f = !f;
                        _endPoint = _currentPoint;
                    }

                    // go back 1 cell
                    path.RemoveAt(path.Count - 1);
                    _currentPoint = path[path.Count - 1];
                }
                else
                {


                    switch (validDirections[_rng.Next(0, validDirections.Count)])
                    {
                        case Directions.North:
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.North);
                            _currentPoint.Y--;
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.South);
                            break;
                        case Directions.South:
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.South);
                            _currentPoint.Y++;
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.North);
                            break;
                        case Directions.East:
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.East);
                            _currentPoint.X++;
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.West);
                            break;
                        case Directions.West:
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.West);
                            _currentPoint.X--;
                            _Board[_currentPoint.X, _currentPoint.Y].RemoveWall(RectCell.Walls.East);
                            break;
                    }

                    path.Add(_currentPoint);
                    _Board[_currentPoint.X, _currentPoint.Y].Visit();
                }
                _iterations++;
            }

        }

        private void ValidateMovement(Point current, List<Directions> validDirections)
        {
            List<Directions> invalidDirections = new List<Directions>();
            foreach (var item in validDirections)
            {
                switch (item)
                {
                    case Directions.North:
                        if (_currentPoint.Y == 0 || Visited(_currentPoint.X, _currentPoint.Y - 1))
                            invalidDirections.Add(Directions.North);
                        break;
                    case Directions.South:
                        if (_currentPoint.Y == _Height - 1 || Visited(_currentPoint.X, _currentPoint.Y + 1))
                            invalidDirections.Add(Directions.South);
                        break;
                    case Directions.East:
                        if (_currentPoint.X == _Width - 1 || Visited(_currentPoint.X + 1, _currentPoint.Y))
                            invalidDirections.Add(Directions.East);
                        break;
                    case Directions.West:
                        if (_currentPoint.X == 0 || Visited(_currentPoint.X - 1, _currentPoint.Y))
                            invalidDirections.Add(Directions.West);
                        break;
                }
            }
            foreach (var item in invalidDirections)
                validDirections.Remove(item);
        }


        private List<Directions> GetAllDirections()
        {
            return new List<Directions>() {

                Directions.North,
                Directions.South,
                Directions.East,
                Directions.West
                };
        }

        private bool Visited(int x, int y)
        {
            return _Board[x, y].Visited;
        }

        private bool allVisited()
        {
            foreach (var item in _Board)
            {
                if (!item.Visited)
                    return false;
            }
            return true;
        }

        public static short[,] GenerateMatrix(Maze maze)
        {
            int width = maze.Width * 3;
            int height = maze.Height * 3;

            short[,] matrix = new short[width, height];



            foreach (var item in maze.Board)
            {  
                //if (item.IsDeadEnd)
                //    matrix[item.Point.X + 1, item.Point.Y + 1] = (short)MapTile.DeadEnd;
                //else if (item.HasLadderUp)
                //    matrix[item.Point.X + 1, item.Point.Y + 1] = (short)MapTile.LadderUp;
                //else if (item.HasLadderDown)
                //    matrix[item.Point.X + 1, item.Point.Y + 1] = (short)MapTile.LadderDown;
                ////else
                ////    matrix[item.Point.X + 1, item.Point.Y + 1] = (short)MapTile.Empty;

                if (item.NorthWall)
                    InsertInRow((short)MapTile.Wall, 3, matrix, new Point(item.Point.X * 3, item.Point.Y * 3));
                if (item.SouthWall)
                    InsertInRow((short)MapTile.Wall, 3, matrix, new Point(item.Point.X * 3, (item.Point.Y * 3) + 2));
                if (item.WestWall)
                    InsertInCol((short)MapTile.Wall, 3, matrix, new Point(item.Point.X * 3, item.Point.Y * 3));
                if (item.EastWall)
                    InsertInCol((short)MapTile.Wall, 3, matrix, new Point((item.Point.X * 3) + 2, item.Point.Y * 3));

            }

            matrix[maze._startPoint.X * 3 + 1, maze._startPoint.Y * 3 + 1] = (short)MapTile.StartCell;
            matrix[maze._endPoint.X * 3 + 1, maze._endPoint.Y * 3 + 1] = (short)MapTile.EndCell;



            return matrix;
        }

        private static void InsertInRow(short value, int n, short[,] matrix, Point pos)
        {
            for (int i = pos.X; i < pos.X + n; i++)
            {
                matrix[i, pos.Y] = value;
            }
        }
        private static void InsertInCol(short value, int n, short[,] matrix, Point pos)
        {
            for (int i = pos.Y; i < pos.Y + n; i++)
            {
                matrix[pos.X, i] = value;
            }
        }
    }
}

