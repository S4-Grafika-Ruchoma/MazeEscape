using System;
using System.Collections.Generic;
using System.Linq;
using MazeEscape.AI;
using MazeEscape.CustomClasses;
using MazeEscape.Enums;
using MazeEscape.GameObjects;
using MazeEscape.MainMenu;
using MazeEscape.MazeGen;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeEscape
{
    public class Game1 : Game
    {
        private MainMenu.MainMenu menu;

        public bool RunGame = AppConfig._DEBUG_SKIP_MAIN_MENU_;
        public bool NotAllCollected { get; set; }

        // Elementy monogame
        private KeyboardState prevState;
        GraphicsDeviceManager graphics;
        BasicEffect basicEffect;
        SpriteBatch spriteBatch;

        // Elementy gry
        public SoundManager soundManager;
        List<List<int>> mapMatrix;
        List<Object3D> gameMap;
        List<Object3D> collectables;
        Camera camera;
        public Enemy enemy;

        Effect _ambientEffect;

        // Generator labiryntu
        Maze mazeGenerator;

        // Licznik FPS i informacje debugowania
        SpriteFont Font;
        SpriteFont TimerFont;
        float time_in_game = 0;
        float intro_time = 0;
        int time_secounds = 0;
        int time_minutes = 0;
        int time_hours = 0; // For Alan...  (*,*)
        int level_counter = 1;
        int box_counter = 0;
        int box_helper = 0;
        int finded_box_counter = 0;
        


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

        private Model Wall, Floor, Portal, Enemy, Collectable;

        EffectParameter lightEffectPointLightPosition, lightEffectPointLightColor, lightEffectPointLightIntensity, lightEffectPointLightRadius, lightEffectPointLightRendered;

        Vector3 MovableLight = new Vector3(10, 10, 10);
        const int MaxLights = 5;
        Vector3[] lightsPositions = new Vector3[MaxLights];
        Vector3[] lightsColors = new Vector3[MaxLights];
        float[] lightIntensities = new float[MaxLights];
        float[] lightRedii = new float[MaxLights];

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1,
                VertexColorEnabled = true,
                LightingEnabled = false,
            };

            // Tworzenie i dodawanie kamery
            camera = new Camera(this, new Vector3(0, 15, 0), Vector3.Zero, 5);
            Components.Add(camera);

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
                    {"excited-sound","Sounds/excited_sound"},
                    {"talk-1","Sounds/angry"},
                    {"talk-2","Sounds/dont_leave"},
                    {"enemy_step","Sounds/enemy_step"},
                    {"player_step","Sounds/enemy_step"},
                    {"menu-btn-hover","Sounds/menu_click"},
                    {"menu-btn-click","Sounds/lose sound 1_0"},
                    {"pick-up","Sounds/zombie-collect"},
                    {"portal","Sounds/portal"}
                }
            );

            menu = new MainMenu.MainMenu(this, soundManager, graphics);

            Font = Content.Load<SpriteFont>("Fonts/SpriteFontPL");
            TimerFont = Content.Load<SpriteFont>("Fonts/TimerFont");


            //Wall, Floor, Portal,Axies,Enemy
            Wall = Content.Load<Model>("Models/Wall_v5");
            Floor = Content.Load<Model>("Models/Floor_Model_v2");
            Portal = Content.Load<Model>("Models/Portal_Model_v2");
            Enemy = Content.Load<Model>("Models/kula_v3");
            // ("Models/stozek");
            Collectable = Content.Load<Model>("Models/Box_model_v3");

            // Tworzenie przeciwnika
            enemy = new Enemy(new Vector3(0, 15, 0), Enemy, this.Content, this.camera, soundManager)
            {
                Scale = new Vector3(0.009f, 0.009f, 0.009f),
                Type = ColliderType.Enemy
            };

            #region Light
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
            lightIntensities[3] = 3f;
            lightIntensities[4] = 2f;

            lightRedii[0] = 15;
            lightRedii[1] = 10;
            lightRedii[2] = 10;
            lightRedii[3] = 50;
            lightRedii[4] = 15;

            if (AppConfig._DEBUG_SUN_)
            {
                lightsPositions[3] = new Vector3(50, 10, 50);
                lightIntensities[3] = 18;
                lightRedii[3] = 120;
            }

            lightEffectPointLightPosition.SetValue(lightsPositions);
            lightEffectPointLightColor.SetValue(lightsColors);
            lightEffectPointLightIntensity.SetValue(lightIntensities);
            lightEffectPointLightRadius.SetValue(lightRedii);
            #endregion

            // Tworzenie mapy i reprezentacji 3D
            GenerateGameMap();

            box_counter = camera.CollectablesObjects.Count;  // Przypisanie ilości znajdziek
            box_helper = box_counter;

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) && prevState.IsKeyUp(Keys.Escape))
            {
                RunGame = !RunGame;
                menu.State = MenuState.MainMenu;
                soundManager.Pause("game-ambient");
                soundManager.Stop("talk-1");
                soundManager.Play("menu-ambient", 0.5f);
            }

            if (!RunGame)
            {
                time_in_game += (float)gameTime.ElapsedGameTime.TotalSeconds;   // Timer
                time_secounds = +(int)time_in_game;

                intro_time += (float)gameTime.ElapsedGameTime.TotalSeconds;     //Intro timer
                if (intro_time > 20) intro_time = 21; 

                if (time_in_game > 60) { time_in_game = 0; }

                if (time_secounds >= 60)
                {
                    time_minutes += 1;
                    time_secounds = 0;
                }
                if (time_minutes >= 60)
                {
                    time_hours += 1;
                    time_minutes = 0;
                }

                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    camera.cameraSpeed = AppConfig._SPRINT_SPEED;
                }
                else if (keyboardState.IsKeyUp(Keys.LeftShift))
                {
                    camera.cameraSpeed = AppConfig._WALK_SPEED;
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

                if (keyboardState.IsKeyDown(Keys.L) && prevState.IsKeyUp(Keys.L))
                {
                    enemy.stepCount = 0;
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

                if (keyboardState.IsKeyDown(Keys.NumPad2))
                {
                    lightRedii[3] -= 1;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad5))
                {
                    lightRedii[3] += 1;
                }

                if (keyboardState.IsKeyDown(Keys.NumPad1))
                {
                    lightIntensities[3] -= 0.5f;
                }
                else if (keyboardState.IsKeyDown(Keys.NumPad4))
                {
                    lightIntensities[3] += 0.5f;
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
                    if (camera.CollectablesObjects.Any() && !AppConfig._DEBUG_DISABLE_COLLECTABLES_CHECK_)
                    {
                        NotAllCollected = true;
                    }
                    else
                    {
                        GenerateGameMap();
                        soundManager.Play("portal", 0.2f);
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

                enemy.Step(pathFinder, gameTime);

                base.Update(gameTime);

                _ambientEffect.Parameters["CameraPosition"].SetValue(camera.Position);

                basicEffect.View = camera.View;
                basicEffect.Projection = camera.Projection;
                basicEffect.World = Matrix.Identity;
            }
            else
            {
                menu.Update(gameTime);
            }
            prevState = keyboardState;

            if (box_counter != camera.CollectablesObjects.Count)  // Logika podnoszenia znajdziek
            {
                finded_box_counter += 1;
                box_counter = camera.CollectablesObjects.Count;
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            if (!RunGame)
            {
                var depthStencilState = new DepthStencilState
                {
                    DepthBufferEnable = true
                };
                GraphicsDevice.DepthStencilState = depthStencilState;

                GraphicsDevice.Clear(Color.Black);

                // Rysowanie mapy
                foreach (var object3D in gameMap)
                {
                    object3D.Draw();
                    if (camera.ShowColliders && object3D is Collider objectCollider)
                    {
                        objectCollider.DrawCollider(basicEffect, GraphicsDevice);
                    }
                }

                foreach (var object3D in collectables.Where(a =>
                    camera.CollectablesObjects.Any(b => a.ColliderBox == b.ColliderBox)))
                {
                    object3D.Draw();
                    if (camera.ShowColliders && object3D is Collider objectCollider)
                    {
                        objectCollider.DrawCollider(basicEffect, GraphicsDevice);
                    }
                }

                enemy.Draw();

                if (camera.ShowColliders)
                {
                    camera.DrawCollider(basicEffect, GraphicsDevice);
                    enemy.DrawCollider(basicEffect, GraphicsDevice);
                }

                spriteBatch.Begin();
                {

                    if (intro_time < 8)
                    {
                        spriteBatch.DrawString(Font, $"Twoim celem jest znalezienie {box_helper} skrzynek",    // Wstęp do gry
                           new Vector2(GraphicsDevice.Viewport.Width / 2 - 250, GraphicsDevice.Viewport.Height / 2 - 180),
                           Color.White, 0, Vector2.Zero,
                           new Vector2(0.5f), SpriteEffects.None, 0);

                        spriteBatch.DrawString(Font, $"ale uważaj nie jesteś tu sam...",                   // Wstęp do gry v2
                           new Vector2(GraphicsDevice.Viewport.Width / 2 - 220, GraphicsDevice.Viewport.Height / 2 - 140),
                           Color.White, 0, Vector2.Zero,
                           new Vector2(0.5f), SpriteEffects.None, 0);
                    }

                    spriteBatch.DrawString(TimerFont, $"{time_hours}:{time_minutes}:{time_secounds}",       // Wyświetlanie czasu gry
                        new Vector2(GraphicsDevice.Viewport.Width / 2 - 30, 15),
                        Color.White, 0, Vector2.Zero,
                        new Vector2(0.5f), SpriteEffects.None, 0);

                    spriteBatch.DrawString(TimerFont, $"Poziom: {level_counter}",                           // Wyświetlanie aktualnego poziomu
                        new Vector2(20, 60),  // Pozycja
                        Color.White, 0, Vector2.Zero,
                        new Vector2(0.4f), SpriteEffects.None, 0);

                    spriteBatch.DrawString(TimerFont, $"Skrzynki: {finded_box_counter} z {box_helper}",    // Wyświetlanie ilości skrzynek
                        new Vector2(20, 90),  // Pozycja
                        Color.White, 0, Vector2.Zero,
                        new Vector2(0.4f), SpriteEffects.None, 0);

                    if (camera.CollectablesObjects.Any(a => a.ColliderBox.Intersects(camera.GrabCollider)))
                    {
                        spriteBatch.DrawString(Font, $"Naciśnij E aby podnieść skrzynkę",
                            new Vector2(GraphicsDevice.Viewport.Width / 2 - 140,
                                GraphicsDevice.Viewport.Height / 2), Color.White, 0, Vector2.Zero,
                            new Vector2(0.4f), SpriteEffects.None, 0);
                    }

                    if (NotAllCollected || AppConfig._DEBUG_DISABLE_COLLECTABLES_CHECK_)
                    {
                        spriteBatch.DrawString(Font, $"Znajdź wszystkie skrzynki!",
                            new Vector2(GraphicsDevice.Viewport.Width / 2 - 170, 200), Color.White, 0, Vector2.Zero, new Vector2(0.6f),
                            SpriteEffects.None, 0);
                    }

                    if (camera.CantGoBack)
                    {
                        spriteBatch.DrawString(Font, $"Nie mogę się cofać...",
                            new Vector2(GraphicsDevice.Viewport.Width / 2 - 180, 150), Color.Red, 0, Vector2.Zero, new Vector2(0.6f),
                            SpriteEffects.None, 0);
                    }

                    float xPos = 5f, yPos = 5f, inc = 25f;
                    totalFrames++;

                    if (camera.NoClip)
                    {
                        #region  lewy panel

                        spriteBatch.DrawString(Font, $"GRACZ FPS:{fps}", new Vector2(xPos, yPos), Color.Green, 0,
                            Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                        yPos += inc;

                        spriteBatch.DrawString(Font,
                            $"Pozycja : {camera.Position.X:F2} x {camera.Position.Y:F2} x {camera.Position.Z:F2}",
                            new Vector2(xPos, yPos),
                            Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                        yPos += inc;

                        spriteBatch.DrawString(Font, $"[M] NoClip: {camera.NoClip}", new Vector2(xPos, yPos),
                            Color.Aqua, 0,
                            Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                        yPos += inc;

                        spriteBatch.DrawString(Font, $"[U] Colliders: {camera.ShowColliders}", new Vector2(xPos, yPos),
                            Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                        yPos += inc;

                        spriteBatch.DrawString(Font, $"[F] Flashlight: {camera.Falshlight}", new Vector2(xPos, yPos),
                            Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                        yPos += inc;

                        spriteBatch.DrawString(Font, $"Znajdziek: {camera.CollectablesObjects.Count}",
                            new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None,
                            0);
                        yPos += inc;

                        #endregion

                        xPos = GraphicsDevice.Viewport.Width - 300;
                        yPos = 5f;
                        inc = 25f;

                        #region prawy panel

                        spriteBatch.DrawString(Font, $"PRZECIWNIK", new Vector2(xPos, yPos), Color.Green, 0,
                            Vector2.Zero,
                            new Vector2(0.3f), SpriteEffects.None, 0);
                        yPos += inc;

                        spriteBatch.DrawString(Font,
                            $"Pozycja : {enemy.Position.X:F2} x {enemy.Position.Y:F2} x {enemy.Position.Z:F2}",
                            new Vector2(xPos, yPos),
                            Color.Green, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                        yPos += inc;

                        spriteBatch.DrawString(Font,
                            $"Timer dzwięku: {enemy.timer} / {soundManager.GetDuration("enemy_step")}",
                            new Vector2(xPos, yPos), Color.Aqua, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None,
                            0);
                        yPos += inc;

                        #endregion
                    }
                }
                spriteBatch.End();

                base.Draw(gameTime);
            }
            else
            {
                menu.Draw(gameTime);
            }
        }


        private void GenerateGameMap()
        {
            // Tworzenie losowej mapy o podanych rozmiarach
            mazeGenerator = new Maze(15, 15);

            // Init musi być najpierw
            mazeGenerator.Initialize();

            // A potem to 
            mazeGenerator.Generate();

            // Usuwacz DeadEnd'ów (spoko wartość 0.1f -> powyżej 0.5f jest pusta chujnia
            //  1.0f wygląda jak żołądek czarnucha - ogólna pustka i kilka kamieni
            mazeGenerator.DeadEndRemover(0.1f);

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

            //mapMatrix = new List<List<int>>()
            //{
            //    new List<int>{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,},
            //    new List<int>{-1,0,0,0,-1,0,0,0,0,0,0,-1,-1,0,0,4,-1,},
            //    new List<int>{-1,0,0,0, -1, 0,0,0,0,0,-1,-1,0,0,0,0,-1,},
            //    new List<int>{-1,0,0,0, -1, 0,0,0,0,0,-1,-1,0,0,0,0,-1,},
            //    new List<int>{-1,0,0,0,0,0,0,0,0,0,0,-1,-1,-1,0,0,-1,},
            //    new List<int>{-1,-1,-1,-1,-1,-1,0,0,0,-1,-1,0,0,0,0,0,-1,},
            //    new List<int>{-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1,},
            //    new List<int>{-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1,},
            //    new List<int>{-1,0,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,},
            //    new List<int>{-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1,},
            //    new List<int>{-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1,},
            //    new List<int>{-1,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,0,0,0,-1,},
            //    new List<int>{-1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1,},
            //    new List<int>{-1,0,-1,0,0,0,0,0,0,0,0,-1,0,0,0,0,-1,},
            //    new List<int>{-1,0,-1,0,0,0,0,0,0,0,0,-1,0,0,0,0,-1,},
            //    new List<int>{-1,5,-1,0,0,0,0,0,0,0,0,-1,0,0,0,0,-1,},
            //    new List<int>{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,},
            //};

            gameMap = new List<Object3D>();
            collectables = new List<Object3D>();

            Random rnd = new Random();

            camera.ResetColiders();

            var pathMap = new List<List<Spot>>();

            var row = 0;
            foreach (var mapRow in mapMatrix)
            {
                pathMap.Add(new List<Spot>());
                var col = 0;
                foreach (var mapCell in mapRow)
                {
                    pathMap[row].Add(new Spot(new Point(row, col), ColliderType.Wall));

                    if (mapCell == (int)MapTile.Wall)
                    {
                        gameMap.Add(new Object3D(Content, camera, Wall)
                        {
                            Position = new Vector3(row * 2, 1, col * 2),
                            Scale = new Vector3(0.01f),
                            Type = ColliderType.Wall,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice
                        });
                        pathMap[row][col].Type = ColliderType.Wall;
                    }
                    else if (mapCell == (int)MapTile.EndCell)
                    {
                        gameMap.Add(new Object3D(Content, camera, Portal)
                        {
                            Position = new Vector3(row * 2, 0.7f, col * 2),
                            Scale = new Vector3(0.01f, 0.01f, 0.1f),
                            Type = ColliderType.LadderExit,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice,
                            RotationAnimation = new Vector3(0,0, 0.01f)
                        });
                        pathMap[row][col].Type = ColliderType.LadderExit;
                        lightsPositions[2] = new Vector3(row * 2, 2, col * 2); // RunGame

                        camera.EndCollider = gameMap.Last().ColliderBox;
                        camera.NextLevelStartPosition = new Vector3(row * 2, 1, col * 2);
                    }
                    else if (mapCell == (int)MapTile.StartCell)
                    {
                        gameMap.Add(new Object3D(Content, camera, Portal)
                        {
                            Position = new Vector3(row * 2, 0.7f, col * 2),
                            Scale = new Vector3(0.01f, 0.01f, 0.1f),
                            Type = ColliderType.LadderEnter,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice,
                            RotationAnimation = new Vector3(0, 0, 0.01f)
                        });
                        pathMap[row][col].Type = ColliderType.LadderEnter;
                        lightsPositions[1] = new Vector3(row * 2, 2, col * 2); // START

                        if (!AppConfig._DEBUG_DISABLE_START_SPAWN_)
                            camera.Position = new Vector3(row * 2, 1, col * 2);
                    }

                    if (mapCell == (int)MapTile.Empty || mapCell == (int)MapTile.StartCell || mapCell == (int)MapTile.EndCell)
                    {
                        gameMap.Add(new Object3D(Content, camera, Floor)
                        {
                            Position = new Vector3(row * 2, -1, col * 2),
                            Scale = new Vector3(0.01f),
                            Type = ColliderType.Floor,
                            lighting = _ambientEffect,
                            GraphicsDevice = GraphicsDevice
                        });

                        if (mapCell == (int)MapTile.StartCell)
                        {
                            pathMap[row][col].Type = ColliderType.LadderEnter;
                        }
                        else if (mapCell == (int)MapTile.EndCell)
                        {
                            pathMap[row][col].Type = ColliderType.LadderExit;
                        }
                        else
                        {
                            pathMap[row][col].Type = ColliderType.Empty;

                            if (rnd.Next(0, 100) > 97)
                            {
                                collectables.Add(new Object3D(Content, camera, Collectable) // Znajdźka
                                {
                                    Position = new Vector3(row * 2, 0.2f, col * 2),
                                    Scale = new Vector3(0.01f),
                                    Type = ColliderType.Collectable,
                                    lighting = _ambientEffect,
                                    GraphicsDevice = GraphicsDevice
                                });
                                pathMap[row][col].Type = ColliderType.Collectable;
                            }
                        }

                    }

                    col++;
                }

                row++;
            }


            NotAllCollected = false;
            camera.Collected = 0;


            camera.AddColliderObject(enemy);
            camera.AddColliderObjects(gameMap.Cast<Collider>().ToList());
            camera.AddColectableObjects(collectables.Cast<Collider>().ToList());

            int Pos1 = 1;//1x1 - 15x15
            int Pos2 = 1;
            do
            {
                Pos1 = rnd.Next(0, mapMatrix.Count - 1);
                Pos2 = rnd.Next(0, mapMatrix.Count - 1);
            }
            while (mapMatrix[Pos1][Pos2] == (int)MapTile.Wall);

            enemy.Position = new Vector3(Pos1 * 2, 1, Pos2 * 2);
            //camera.AddColliderObject(enemy.ColliderBox);

            pathFinder = new Pathfinding(pathMap, enemy);
        }

        private Pathfinding pathFinder;
    }
}
