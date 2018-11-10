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
        List<Object3D> collectables;
        Object3D cameraAxies;
        Camera camera;
        Floor floor;
        public Enemy enemy;

        Effect _ambientEffect;

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

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        EffectParameter lightEffectPointLightPosition, lightEffectPointLightColor, lightEffectPointLightIntensity, lightEffectPointLightRadius, lightEffectPointLightRendered;

        Vector3 MovableLight = new Vector3(-10, -10, -10);
        const int MaxLights = 5;
        Vector3[] lightsPositions = new Vector3[MaxLights];
        Vector3[] lightsColors = new Vector3[MaxLights];
        float[] lightIntensities = new float[MaxLights];
        float[] lightRedii = new float[MaxLights];

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
            floor = new Floor(GraphicsDevice, 90, 90);
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
            mazeGenerator = new Maze(15, 15);


            _ambientEffect = Content.Load<Effect>("Effects/Test");
            _ambientEffect.Parameters["SunLightColor"].SetValue(Color.Red.ToVector3());
            _ambientEffect.Parameters["SunLightDirection"].SetValue(Vector2.Zero);
            _ambientEffect.Parameters["SunLightIntensity"].SetValue(0.1f);

            lightEffectPointLightPosition = _ambientEffect.Parameters["PointLightPosition"];
            lightEffectPointLightColor = _ambientEffect.Parameters["PointLightColor"];
            lightEffectPointLightIntensity = _ambientEffect.Parameters["PointLightIntensity"];

            lightEffectPointLightRadius = _ambientEffect.Parameters["PointLightRadius"];
            lightEffectPointLightRendered = _ambientEffect.Parameters["MaxLightsRendered"];

            lightEffectPointLightRendered.SetValue(MaxLights);

            lightsPositions[0] = enemy.Position;
            //lightsPositions[1] = new Vector3(80, 5, 80); // Lader in
            //lightsPositions[2] = new Vector3(60, 5, 60); // Ladder out
            lightsPositions[3] = MovableLight;
            lightsPositions[4] = camera.Position;

            lightsColors[0] = Color.Red.ToVector3();
            lightsColors[1] = Color.Green.ToVector3();
            lightsColors[2] = Color.Blue.ToVector3();
            lightsColors[3] = Color.Wheat.ToVector3();
            lightsColors[4] = Color.Wheat.ToVector3();

            lightIntensities[0] = 2f;
            lightIntensities[1] = 2f;
            lightIntensities[2] = 2f;
            lightIntensities[3] = 2f;
            lightIntensities[4] = 2f;

            lightRedii[0] = 15;
            lightRedii[1] = 10;
            lightRedii[2] = 10;
            lightRedii[3] = 15;
            lightRedii[4] = 15;

            lightEffectPointLightPosition.SetValue(lightsPositions);
            lightEffectPointLightColor.SetValue(lightsColors);
            lightEffectPointLightIntensity.SetValue(lightIntensities);
            lightEffectPointLightRadius.SetValue(lightRedii);

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

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                MovableLight += new Vector3(0, 0, 0.2f);
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                MovableLight -= new Vector3(0, 0, 0.2f);
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                MovableLight += new Vector3(0.2f, 0, 0);
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                MovableLight -= new Vector3(0.2f, 0, 0);
            }
            
            if (keyboardState.IsKeyDown(Keys.PageUp))
            {
                MovableLight += new Vector3(0, 0.2f, 0);
            }
            else if (keyboardState.IsKeyDown(Keys.PageDown))
            {
                MovableLight -= new Vector3(0, 0.2f, 0);
            }

            if (camera.Falshlight)
            {
                lightIntensities[4] = 2f;
            }
            else
            {
                lightIntensities[4] = 0;
            }

            lightsPositions[0] = enemy.Position;
            lightsPositions[3] = MovableLight;
            lightsPositions[4] = camera.Position;

            lightEffectPointLightPosition.SetValue(lightsPositions);
            lightEffectPointLightColor.SetValue(lightsColors);
            lightEffectPointLightIntensity.SetValue(lightIntensities);
            lightEffectPointLightRadius.SetValue(lightRedii);

            if (camera.IsEndLevelCollision())
            {
                if (camera.CollectablesObjects.Any())
                {
                    NotAllCollected = true;
                }
                else
                {
                    GenerateGameMap();
                }
            }
            else
            {
                NotAllCollected = false;
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


            _ambientEffect.Parameters["CameraPosition"].SetValue(camera.Position);
        }

        public bool NotAllCollected { get; set; }

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
                CullMode = CullMode.CullCounterClockwiseFace
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
            foreach (var object3D in collectables.Where(a=>camera.CollectablesObjects.Any(b=>a.ColliderBox==b)))
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
                DrawCollider(camera.GrabCollider);
            }

            spriteBatch.Begin();
            {
                if (camera.CollectablesObjects.Any(a => a.Intersects(camera.GrabCollider)))
                {
                    spriteBatch.DrawString(Font, $"Podnieś znajdźke", new Vector2(GraphicsDevice.Viewport.Width/2-50, GraphicsDevice.Viewport.Height/2-50), Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                }

                if (NotAllCollected)
                {
                    spriteBatch.DrawString(Font, $"Żeby przejść dalej muszisz znaleść wszystkie cosie....", new Vector2(GraphicsDevice.Viewport.Width / 2 - 250, GraphicsDevice.Viewport.Height / 2 - 20), Color.Red, 0, Vector2.Zero, new Vector2(0.4f), SpriteEffects.None, 0);
                }

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

                spriteBatch.DrawString(Font, $"[F] Flashlight: {camera.Falshlight}", new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Znajdziek: {camera.CollectablesObjects.Count}", new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                yPos += inc;

                spriteBatch.DrawString(Font, $"Znajdzkia: {camera.CollectablesObjects.Any(a => a.Intersects(camera.GrabCollider))}", new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
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


        public void DrawCollider(BoundingBox ColliderBox)
        {
            using (var line = new Line())
            {
                var n = ColliderBox.Min;
                var x = ColliderBox.Max;

                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(n.X, n.Y, x.Z), Color.Red); // N -> N+X.z 1-2
                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(n.X, x.Y, n.Z), Color.Red); // N -> N+X.y 1-3
                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(x.X, n.Y, n.Z), Color.Red); // N -> N+X.z 1-4

                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(n.X, x.Y, x.Z), Color.Blue); // X -> X+N.x 5-6
                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(x.X, n.Y, x.Z), Color.Blue); // X -> X+N.y 5-7
                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(x.X, x.Y, n.Z), Color.Blue); // X -> X+N.z 5-8

                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, n.Y, n.Z), new Vector3(x.X, n.Y, x.Z),
                    Color.Salmon); // 4-7
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, n.Y, x.Z), new Vector3(n.X, n.Y, x.Z),
                    Color.Salmon); // 7-2
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, n.Y, x.Z), new Vector3(n.X, x.Y, x.Z),
                    Color.Salmon); // 2-6

                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, x.Y, x.Z), new Vector3(n.X, x.Y, n.Z),
                    Color.Cyan); // 6-3
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, x.Y, n.Z), new Vector3(x.X, x.Y, n.Z),
                    Color.Cyan); // 3-8
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, x.Y, n.Z), new Vector3(x.X, n.Y, n.Z),
                    Color.Cyan); // 8-4
            }
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
                Content.Load<Model>("Models/TestTexturNaModelach_v2"),
                //Content.Load<Model>("Models/TestTexturNaModelach"),
                //Content.Load<Model>("Models/wallBlock"),
                //Content.Load<Model>("Models/Wall_Block_v1a"),
                //Content.Load<Model>("Models/Wall_Block_v2a")
            };

            var ladder = Content.Load<Model>("Models/ladder");

            gameMap = new List<Object3D>();
            collectables = new List<Object3D>();

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
                            Type = ColliderType.Wall,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice
                        });
                    }
                    else if (mapCell == (int)MapTile.EndCell)
                    {
                        gameMap.Add(new Object3D(Content, camera, wallBlock[rnd.Next(0, wallBlock.Count())] /*ladder*/)
                        {
                            Position = new Vector3(row * 2, 2, col * 2),
                            Scale = new Vector3(0.01f, 0.01f, 0.1f),
                            Type = ColliderType.LadderExit,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice
                        });
                        lightsPositions[2] = new Vector3(row * 2, 2, col * 2); // STOP

                        camera.EndCollider = gameMap.Last().ColliderBox;
                        camera.NextLevelStartPosition = new Vector3(row * 2, 1, col * 2);
                    }
                    else if (mapCell == (int)MapTile.StartCell)
                    {
                        gameMap.Add(new Object3D(Content, camera, wallBlock[rnd.Next(0, wallBlock.Count())]/*ladder*/ )
                        {
                            Position = new Vector3(row * 2, 2, col * 2),
                            Scale = new Vector3(0.01f, 0.01f, 0.1f),
                            Type = ColliderType.LadderEnter,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice
                        });
                        lightsPositions[1] = new Vector3(row * 2, 2, col * 2); // START

                        if (!AppConfig._DEBUG_DISABLE_START_SPAWN_)
                            camera.Position = new Vector3(row * 2, 1, col * 2);
                    }else if (rnd.Next(0, 100) > 97)
                    {
                        collectables.Add(new Object3D(Content, camera, wallBlock[rnd.Next(0, wallBlock.Count())] /*collectable*/)
                        {
                            Position = new Vector3(row * 2, 0, col * 2),
                            Scale = new Vector3(0.002f),
                            Type = ColliderType.Collectable,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice
                        });
                    }

                    col++;
                }

                row++;
            }

            NotAllCollected = false;
            camera.Collected = 0;
            camera.AddColliderObject(enemy.ColliderBox);
            camera.AddColliderObjects(gameMap.Where(a => a.Type != ColliderType.LadderEnter).Select(a => a.ColliderBox).ToList());

            camera.AddColectableObjects(collectables.Select(a => a.ColliderBox).ToList());

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
