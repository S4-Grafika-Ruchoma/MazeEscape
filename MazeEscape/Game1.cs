using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MazeEscape.Interfaces;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjektTestowy.CustomClasses;
using Sounds;

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

        private List<Object3D> obj;
        private List<Line> lines;

        List<List<List<int>>> map;
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

            floor = new Floor(GraphicsDevice, 50, 50);

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

            var wallBlock = Content.Load<Model>("Models/wallBlock");
            var ladder = Content.Load<Model>("Models/ladder");

            obj = new List<Object3D>();

            var floorLevel = 0;
            foreach (var level in map)
            {
                var row = 0;
                foreach (var mapRow in level)
                {
                    var col = 0;
                    foreach (var mapCell in mapRow)
                    {
                        if (mapCell == 1)
                        {
                            obj.Add(new Object3D(Content, camera, wallBlock)
                            {
                                Position = new Vector3(row * 2, floorLevel, col * 2),
                                Scale = new Vector3(0.01f),
                            });
                        }
                        else if (mapCell == 2)
                        {
                            obj.Add(new Object3D(Content, camera, ladder)
                            {
                                Position = new Vector3(row * 2, floorLevel, col * 2),
                                Scale = new Vector3(0.01f),
                            });
                        }

                        col++;
                    }

                    row++;
                }

                floorLevel += 2;
            }
            camera.Map = obj;

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
            floorList = new List<List<int>>(){
                new List<int>() {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 1, 1 },
        };

            map = new List<List<List<int>>>() // 0 korytarz // 1 - ściana // 2 - drabina // 
        {
            new List<List<int>>(){
                new List<int>() {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1},
                new List<int>() {1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1 ,1, 0, 0, 1, 1, 0, 1},
                new List<int>() {1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0 ,1, 1, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0 ,0, 1, 0, 1 },
                new List<int>() {1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1},
                new List<int>() {1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 2, 1},
                new List<int>() {1, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1},
                new List<int>() {1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1},
                new List<int>() {1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1},
                new List<int>() {1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
                new List<int>() {1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1 ,0, 1, 0, 1},
                new List<int>() {1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1 ,0, 1, 0, 1 },
                new List<int>() {1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1 ,0, 1, 0, 1 },
                new List<int>() {1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0 ,0, 1, 0, 1 },
                new List<int>() {1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1 ,1, 1, 0, 1 },
                new List<int>() {1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 ,0, 1, 0, 1 },
                new List<int>() {1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1 ,0, 1, 0, 1 },
                new List<int>() {1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1 ,0, 1, 0, 1 },
                new List<int>() {1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 ,1, 1, 0, 1 },
                new List<int>() {1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1 ,1, 1, 0, 1 },
                new List<int>() {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 1, 1 },
            },
            floorList,
            new List<List<int>>(){
                new List<int>() {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1},
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0, 0, 1 },
                new List<int>() {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 1, 1 },
            },
            floorList,
        };
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spr_font = Content.Load<SpriteFont>("Fonts/SpriteFontPL");
        }

        protected override void UnloadContent() { }

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

            //if (keyboardState.IsKeyDown(Keys.LeftControl))
            //{
            //    camera.Position = new Vector3(camera.Position.X, 0.5f, camera.Position.Z);
            //}
            //else if (keyboardState.IsKeyUp(Keys.LeftControl))
            //{
            //    camera.Position = new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z);
            //}

            if (gamepadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (keyboardState.IsKeyDown(Keys.M))
            {
                camera.AllowClimb = !camera.AllowClimb;
            }

            if (keyboardState.IsKeyDown(Keys.U))
            {
                camera.ShowColliders = !camera.ShowColliders;
            }

            if (keyboardState.IsKeyDown(Keys.J))
            {
                camera.ShowCenterLine = !camera.ShowCenterLine;
            }

            // Update
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var depthStencilState = new DepthStencilState
            {
                DepthBufferEnable = true
            };
            GraphicsDevice.DepthStencilState = depthStencilState;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            floor.Draw(camera, basicEffect);


            RasterizerState rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (var object3D in obj)
            {
                object3D.Draw();
                if (camera.ShowColliders)
                    DrawBoundingBoxesLines(object3D.Collider);
            }

            cameraAxies.Position = camera.cameraAxiesPosition;
            cameraAxies.Draw();

            foreach (var line in lines)
            {
                line.DrawLine(basicEffect, GraphicsDevice);
            }
            if (camera.ShowCenterLine)
                playerLine.DrawLine(basicEffect, GraphicsDevice, Vector3.Zero, new Vector3(camera.Position.X, camera.Position.Y - 0.02f, camera.Position.Z));

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

            if(camera.ShowColliders)
            DrawBoundingBoxesLines( camera.Collider);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawBoundingBoxesLines(BoundingBox box)
        {
            using (var line = new Line())
            {

                var n = box.Min;
                var x = box.Max;

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
    }


    class Line : IDisposable
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        private VertexPositionColor[] vertices;

        public Line(Vector3 startPoint, Vector3 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public Line() { }

        public void DrawLine(BasicEffect basicEffect, GraphicsDevice graphicsDevice)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices = new[] { new VertexPositionColor(startPoint, Color.White), new VertexPositionColor(endPoint, Color.White) };
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public void DrawLine(BasicEffect basicEffect, GraphicsDevice graphicsDevice, Vector3 start, Vector3 end)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices = new[] { new VertexPositionColor(start, Color.White), new VertexPositionColor(end, Color.White) };
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public void DrawLine(BasicEffect basicEffect, GraphicsDevice graphicsDevice, Vector3 start, Vector3 end, Color color)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices = new[] { new VertexPositionColor(start, color), new VertexPositionColor(end, color) };
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public void Dispose()
        {
        }
    }
}
