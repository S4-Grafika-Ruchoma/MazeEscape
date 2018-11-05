using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MazeEscape.AI;
using MazeEscape.CustomClasses;
using MazeEscape.Enums;
using MazeEscape.GameObjects;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MazeEscape
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Camera camera;
        private Floor floor;
        private BasicEffect basicEffect;
        private SpriteBatch spriteBatch;
        Object3D cameraAxies;
		Enemy enemy;

        MazeGen.Maze Maze;

        private List<Object3D> obj;
        private List<Line> lines;

        List<List<int>> map;
        List<List<int>> floorList;

        SpriteFont _spr_font;
        private int _total_frames = 0, _fps = 0;
        float _elapsed_time = 0.0f;
        private Line playerLine;
        private SoundManager soundMgr;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = AppConfig.ALLOW_RESIZING;
            graphics.PreferredBackBufferHeight = AppConfig.HEIGHT;
            graphics.PreferredBackBufferWidth = AppConfig.WIDTH;
            graphics.IsFullScreen = AppConfig.FULL_SCREEN;
            IsMouseVisible = AppConfig.IS_MOUSE_VISIBLE;
        }

        protected override void Initialize()
        {

            camera = new Camera(this, new Vector3(0, 15, 0), Vector3.Zero, 5);
            Components.Add(camera);

			// Tworzenie przeciwnika
			enemy = new Enemy(new Vector3(0, 10, 0), Content.Load<Model>("Models/stozek"), this.Content, this.camera)
			{
				Scale = new Vector3(0.01f, 0.1f, 0.01f)
			};
			floor = new Floor(GraphicsDevice, 120, 120);
            camera.AddColliderObject(floor.ColliderBox);

            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1,
                VertexColorEnabled = true,
                LightingEnabled = false,
            };

            playerLine = new Line(new Vector3(0, 1, -50), new Vector3(0, 1, 0));
            lines = new List<Line>()
            {
                //new Line(new Vector3(0,0,0), new Vector3(30,10,30)),
                //new Line(new Vector3(0,1,-50), new Vector3(0,1,0))
            };
            _INIT_TEST_MAP_();


            cameraAxies = new Object3D(Content, camera, "Models/axies")
            {
                Scale = new Vector3(0.01f),
                Rotation = new Vector3(MathHelper.ToRadians(-90), MathHelper.ToRadians(180), 0)
            };
            //obj.Add(new Object3D(Content, camera, "Models/axies")
            //{
            //    Position = new Vector3(1, 0, 1),
            //    Scale = new Vector3(1.5f),
            //    Rotation = new Vector3(MathHelper.ToRadians(-90), MathHelper.ToRadians(180), 0)
            //});

            soundMgr = new SoundManager(Content);
            soundMgr.Add(
                new Dictionary<string, string>()
                {
                    {"menu-ambient","Music/menu" }
                },
                new Dictionary<string, string>()
                {
                    {" ","Sounds/menu_click"},
                    {"menu-btn-click","Sounds/lose sound 1_0"},
                    {"talk-1","Sounds/angry"},
                    {"talk-2","Sounds/dont_leave"},
                }
            );

            soundMgr.Play("talk-1");
            //soundMgr.Play("talk-2");

            base.Initialize();
        }

        private void _INIT_TEST_MAP_()
        {
            //Maze generator
            //Jest od chuja różnych konstruktorów
            //najprosszy robi wszystko randomowo
            Maze = new MazeGen.Maze(20, 20);
            //Init musi być najpierw
            Maze.Initialize();
            Maze.Generate();
            // Matrix opisujący co gdzie jest w Mazie
            short[,] Matrix = MazeGen.Maze.GenerateMatrix(Maze);
            //to co oznacza poszczególna liczba jest w enumie
            var x1 = MazeGen.Maze.Matrix.DeadEnd;
            // ten matrix jest jeszcze lekko zdupiony bo wewnętrzne ściany są podwójne


            map = new List<List<int>>();

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                map.Add(new List<int>());
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    map[i].Add(Matrix[i, j]);
                }
            }


            var wallBlock = Content.Load<Model>("Models/Wall_Block_v2a");
            var ladder = Content.Load<Model>("Models/ladder");

            obj = new List<Object3D>();


            var row = 0;
            foreach (var mapRow in map)
            {
                var col = 0;
                foreach (var mapCell in mapRow)
                {
                    if (mapCell == (int)MazeGen.Maze.Matrix.Wall)
                    {
                        obj.Add(new Object3D(Content, camera, wallBlock)
                        {
                            Position = new Vector3(row * 2, 0, col * 2),
                            Scale = new Vector3(0.01f),
                            Type = ColliderType.Wall
                        });
                    }
                    else if (mapCell == (int)MazeGen.Maze.Matrix.EndCell)
                    {
                        obj.Add(new Object3D(Content, camera, ladder)
                        {
                            Position = new Vector3(row * 2, 0, col * 2),
                            Scale = new Vector3(0.03f),
                            Type = ColliderType.LadderExit
                        });
                    }
                    else if (mapCell == (int)MazeGen.Maze.Matrix.StartCell)
                    {
                        obj.Add(new Object3D(Content, camera, ladder)
                        {
                            Position = new Vector3(row * 2, 0, col * 2),
                            Scale = new Vector3(0.04f),
                            Type = ColliderType.LadderEnter
                        });
                    }

                    col++;
                }

                row++;
            }

            camera.ResetColiders();
            camera.AddColliderObjects(obj.Select(a => a.ColliderBox).ToList());

			Random rnd = new Random();
			int Pos1 = 0;
			int Pos2 = 0;
			do
			{
				Pos1 = rnd.Next(0, map.Count - 1);
				Pos2 = rnd.Next(0, map.Count - 1);
			}
			while (map[Pos1][Pos2] == (int)MazeGen.Maze.Matrix.Wall);
			enemy.Position = new Vector3(Pos1 * 2, 1, Pos2 * 2);
		//	camera.AddColliderObject(enemy.ColliderBox);
		}

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spr_font = Content.Load<SpriteFont>("Fonts/SpriteFontPL");
        }

        protected override void UnloadContent() { }

        private KeyboardState prevState;

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var gamepadState = GamePad.GetState(PlayerIndex.One);
            var mousePostion = new Point(mouseState.X, mouseState.Y);

            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                camera.cameraSpeed = 20;
            }
            else if (keyboardState.IsKeyUp(Keys.LeftShift))
            {
                camera.cameraSpeed = 5;
            }

            if (gamepadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            #region Przyciski
            if (keyboardState.IsKeyDown(Keys.M) && prevState.IsKeyUp(Keys.M))
            {
                camera.AllowClimb = !camera.AllowClimb;
            }

            if (keyboardState.IsKeyDown(Keys.U) && prevState.IsKeyUp(Keys.U))
            {
                camera.ShowColliders = !camera.ShowColliders;
            }

            if (keyboardState.IsKeyDown(Keys.J) && prevState.IsKeyUp(Keys.J))
            {
                camera.ShowCenterLine = !camera.ShowCenterLine;
            }

            if (keyboardState.IsKeyDown(Keys.L) && prevState.IsKeyUp(Keys.L))
            {
                _INIT_TEST_MAP_();
            }
            #endregion

            // Update
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }

            prevState = keyboardState;

			enemy.Step();
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            var depthStencilState = new DepthStencilState
            {
                DepthBufferEnable = true
            };
            GraphicsDevice.DepthStencilState = depthStencilState;

            GraphicsDevice.Clear(Color.Black);

            floor.Draw(camera, basicEffect);


            RasterizerState rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (var object3D in obj)
            {
                object3D.Draw();
                if (camera.ShowColliders && object3D is Collider objectCollider)
                {
                    objectCollider.DrawCollider(basicEffect, GraphicsDevice);
                }
            }

            cameraAxies.Position = camera.cameraAxiesPosition;
            cameraAxies.Draw();

            foreach (var line in lines)
            {
                line.DrawLine(basicEffect, GraphicsDevice);
            }
            if (camera.ShowCenterLine)
                playerLine.DrawLine(basicEffect, GraphicsDevice, Vector3.Zero, new Vector3(camera.Position.X, camera.Position.Y - 0.02f, camera.Position.Z));

			enemy.EnemyPlayerLine.DrawLine(basicEffect, GraphicsDevice, enemy.Position, camera.Position);
			spriteBatch.Begin();
            {
                float xPos = 5f, inc = 25f;
                _total_frames++;

                spriteBatch.DrawString(_spr_font, $"FPS:{_fps}", new Vector2(5f, xPos), Color.Indigo, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                xPos += inc;

                spriteBatch.DrawString(_spr_font, $"Pozycja X: {camera.Position.X:F2}", new Vector2(5f, xPos), Color.DarkRed, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                xPos += inc;

                spriteBatch.DrawString(_spr_font, $"Pozycja Y: {camera.Position.Y:F2}", new Vector2(5f, xPos), Color.DarkGreen, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                xPos += inc;

                spriteBatch.DrawString(_spr_font, $"Pozycja Z: {camera.Position.Z:F2}", new Vector2(5f, xPos), Color.DarkBlue, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                xPos += inc;

                spriteBatch.DrawString(_spr_font, $"[M] NoClip: {camera.AllowClimb}", new Vector2(5f, xPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                xPos += inc;

                spriteBatch.DrawString(_spr_font, $"[J] Center line: {camera.ShowCenterLine}", new Vector2(5f, xPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                xPos += inc;

                spriteBatch.DrawString(_spr_font, $"[U] Show colliders: {camera.ShowColliders}", new Vector2(5f, xPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                xPos += inc;

            }

            if (camera.ShowColliders)
            {
                camera.DrawCollider(basicEffect, GraphicsDevice);
                floor.DrawCollider(basicEffect, GraphicsDevice);
				enemy.DrawCollider(basicEffect, GraphicsDevice);
            }


			enemy.Draw();
			

				spriteBatch.End();


            base.Draw(gameTime);
        }
    }


}
