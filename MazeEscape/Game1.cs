using System;
using System.Collections.Generic;
using System.Linq;
using MazeEscape.AI;
using MazeEscape.CustomClasses;
using MazeEscape.Enums;
using MazeEscape.GameObjects;
using MazeEscape.MazeGen;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeEscape
{
    public class Game1 : Game
    {
        // Elementy monogame
        private KeyboardState prevState;
        GraphicsDeviceManager graphics;
        BasicEffect basicEffect;
        SpriteBatch spriteBatch;

        // Elementy gry
        SoundManager soundManager;
        List<List<int>> mapMatrix;
        List<Object3D> gameMap;
        Object3D cameraAxies;
        Camera camera;
        Floor floor;
        Enemy enemy;
        // List<Collider> colliders;

        // Generator labiryntu
        Maze mazeGenerator;

        // Licznik FPS i informacje debugowania
        SpriteFont Font;
        int totalFrames, fps;
        float elapsedTime;

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
            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1,
                VertexColorEnabled = true,
                LightingEnabled = false,
            };

            // Tworzenie i dodawanie kamery
            camera = new Camera(this, new Vector3(0, 15, 0), Vector3.Zero, 5);
            Components.Add(camera);

            // Tworzenie podłogi i dodanie collidera
            floor = new Floor(GraphicsDevice, 120, 120);
            camera.AddColliderObject(floor.ColliderBox);
            
            //Wyświetlenie kierunków XYZ świata
            cameraAxies = new Object3D(Content, camera, "Models/axies")
            {
                Scale = new Vector3(0.01f),
                Rotation = new Vector3(MathHelper.ToRadians(-90), MathHelper.ToRadians(180), 0)
            };

            // Tworzenie menagera dzwięków i dodawanie
            soundManager = new SoundManager(Content);
            soundManager.Add(
                // Song
                new Dictionary<string, string>()
                {
                    {"menu-ambient","Music/menu" },
                    {"game-ambient","Sounds/horror_ambient" }
                },
                // SoundEffect
                new Dictionary<string, string>()
                {
                    {" ","Sounds/menu_click"},
                    {"menu-btn-click","Sounds/lose sound 1_0"},
                    {"talk-1","Sounds/angry"},
                    {"talk-2","Sounds/dont_leave"},
                    {"enemy_step","Sounds/enemy_step"}
                }
            );

            // Tworzenie przeciwnika
            enemy = new Enemy(new Vector3(0, 15, 0), Content.Load<Model>("Models/stozek"), this.Content, this.camera, soundManager)
            {
                Scale = new Vector3(0.01f, 0.05f, 0.01f)
            };

            // Tworzenie losowej mapy o podanych rozmiarach
            mazeGenerator = new Maze(20, 20);

            // Tworzenie mapy i reprezentacji 3D
            GenerateGameMap();

            // Odtworzenie dzwięków
            soundManager.Play("talk-1");
            soundManager.Play("game-ambient");

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("Fonts/SpriteFontPL");
        }

        protected override void UnloadContent() { }
        
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var mousePostion = new Point(mouseState.X, mouseState.Y);

            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                camera.cameraSpeed = 20;
            }
            else if (keyboardState.IsKeyUp(Keys.LeftShift))
            {
                camera.cameraSpeed = 5;
            }

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            #region Przyciski
            if (keyboardState.IsKeyDown(Keys.M) && prevState.IsKeyUp(Keys.M))
            {
                camera.NoClip = !camera.NoClip;
            }

            if (keyboardState.IsKeyDown(Keys.U) && prevState.IsKeyUp(Keys.U))
            {
                camera.ShowColliders = !camera.ShowColliders;
            }

            if (keyboardState.IsKeyDown(Keys.J) && prevState.IsKeyUp(Keys.J))
            {
                camera.ShowLines = !camera.ShowLines;
            }

            if (keyboardState.IsKeyDown(Keys.L) && prevState.IsKeyUp(Keys.L))
            {
                GenerateGameMap();
            }
            #endregion

            if (camera.IsEndLevelCollision())
            {
                GenerateGameMap();
            }

            // Licznik FPS
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime >= 1000.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = 0;
            }

            prevState = keyboardState;

            enemy.Step(gameTime);
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

            var rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

            // Rysowanie mapy
            foreach (var object3D in gameMap)
            {
                object3D.Draw();
                if (camera.ShowColliders && object3D is Collider objectCollider)
                {
                    objectCollider.DrawCollider(basicEffect, GraphicsDevice);
                }
            }

            enemy.Draw();

            // Rysowanie osi kierunków świata
            cameraAxies.Position = camera.cameraAxiesPosition;
            cameraAxies.Draw();

            if (camera.ShowLines)
            {
                enemy.EnemyPlayerLine.DrawLine(basicEffect, GraphicsDevice, enemy.Position, new Vector3(camera.Position.X, camera.Position.Y - 0.02f, camera.Position.Z));
            }

            if (camera.ShowColliders)
            {
                camera.DrawCollider(basicEffect, GraphicsDevice);
                floor.DrawCollider(basicEffect, GraphicsDevice);
                enemy.DrawCollider(basicEffect, GraphicsDevice);
            }

            spriteBatch.Begin();
            {
                float xPos = 5f, yPos = 5f, inc = 25f;
                totalFrames++;

                #region  lewy panel
                spriteBatch.DrawString(Font, $"GRACZ FPS:{fps}", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Pozycja X: {camera.Position.X:F2}", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Pozycja Y: {camera.Position.Y:F2}", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Pozycja Z: {camera.Position.Z:F2}", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"[M] NoClip: {camera.NoClip}", new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"[J] Linie: {camera.ShowLines}", new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"[U] Colliders: {camera.ShowColliders}", new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;
                #endregion

                xPos = GraphicsDevice.Viewport.Width - 250;
                yPos = 5f;
                inc = 25f;

                #region prawy panel
                spriteBatch.DrawString(Font, $"PRZECIWNIK", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Pozycja X: {enemy.Position.X:F2}", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Pozycja Y: {enemy.Position.Y:F2}", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Pozycja Z: {enemy.Position.Z:F2}", new Vector2(xPos, yPos), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Timer dzwięku: {enemy.timer} / {soundManager.GetDuration("enemy_step")}", new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                #endregion
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void GenerateGameMap()
        {
            // Init musi być najpierw
            mazeGenerator.Initialize();

            // A potem to 
            mazeGenerator.Generate();

            // MapTile opisujący co gdzie jest w Mazie
            short[,] Matrix = Maze.GenerateMatrix(mazeGenerator);

            mapMatrix = new List<List<int>>();
            for (var i = 0; i < Matrix.GetLength(0); i++)
            {
                mapMatrix.Add(new List<int>());
                for (var j = 0; j < Matrix.GetLength(1); j++)
                {
                    mapMatrix[i].Add(Matrix[i, j]);
                }
            }

            var wallBlock = new List<Model>()
            {
                Content.Load<Model>("Models/Wall_Block_v1a"),
                Content.Load<Model>("Models/Wall_Block_v2a")
            };

            var ladder = Content.Load<Model>("Models/ladder");

            gameMap = new List<Object3D>();

            Random rnd = new Random();

            camera.ResetColiders();

            var row = 0;
            foreach (var mapRow in mapMatrix)
            {
                var col = 0;
                foreach (var mapCell in mapRow)
                {
                    if (mapCell == (int)MapTile.Wall)
                    {
                        gameMap.Add(new Object3D(Content, camera, wallBlock[rnd.Next(0, wallBlock.Count())])
                        {
                            Position = new Vector3(row * 2, 1, col * 2),
                            Scale = new Vector3(0.01f),
                            Type = ColliderType.Wall
                        });
                    }
                    else if (mapCell == (int)MapTile.EndCell)
                    {
                        gameMap.Add(new Object3D(Content, camera, ladder)
                        {
                            Position = new Vector3(row * 2, 2, col * 2),
                            Scale = new Vector3(0.03f),
                            Type = ColliderType.LadderExit
                        });

                        camera.EndCollider = gameMap.Last().ColliderBox;
                        camera.NextLevelStartPosition = new Vector3(row * 2, 1, col * 2);
                    }
                    else if (mapCell == (int)MapTile.StartCell)
                    {
                        gameMap.Add(new Object3D(Content, camera, ladder)
                        {
                            Position = new Vector3(row * 2, 2, col * 2),
                            Scale = new Vector3(0.04f),
                            Type = ColliderType.LadderEnter
                        });
                        if (!AppConfig._DEBUG_DISABLE_START_SPAWN_)
                            camera.Position = new Vector3(row * 2, 1, col * 2);
                    }

                    col++;
                }

                row++;
            }

            camera.AddColliderObjects(gameMap.Where(a => a.Type != ColliderType.LadderEnter).Select(a => a.ColliderBox).ToList());

            int Pos1 = 0;
            int Pos2 = 0;
            do
            {
                Pos1 = rnd.Next(0, mapMatrix.Count - 1);
                Pos2 = rnd.Next(0, mapMatrix.Count - 1);
            }
            while (mapMatrix[Pos1][Pos2] == (int)MapTile.Wall);
            enemy.Position = new Vector3(Pos1 * 2, 1, Pos2 * 2);
            //camera.AddColliderObject(enemy.ColliderBox);
        }
    }
}
