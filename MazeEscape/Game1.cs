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
        GraphicsDeviceManager graphics;
        private Camera camera;
        private Floor floor;
        private BasicEffect basicEffect;

        TestObject testObj;

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
            camera = new Camera(this, new Vector3(10, 1, 5), Vector3.Zero, 5);
            Components.Add(camera);

            floor = new Floor(GraphicsDevice, 50, 50);

            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1,
                VertexColorEnabled = true,
                LightingEnabled = false,
            };

            base.Initialize();
        }

        private Model cone;

        protected override void LoadContent()
        {
            testObj = new TestObject
            {
                triangleVertices = new[]
                {
                    new VertexPositionColor(new Vector3(0, 20, 0), Color.Red),
                    new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green),
                    new VertexPositionColor(new Vector3(20, -20, 0), Color.Blue),
                },
                vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly),
            };
            testObj.vertexBuffer.SetData<VertexPositionColor>(testObj.triangleVertices);
        }

        protected override void UnloadContent()
        {
        }


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

            if (keyboardState.IsKeyDown(Keys.M))
            {
                camera.AllowClimb = !camera.AllowClimb;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            floor.Draw(camera, basicEffect);

            GraphicsDevice.SetVertexBuffer(testObj.vertexBuffer);
            RasterizerState rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;
            foreach (var item in basicEffect.CurrentTechnique.Passes)
            {
                item.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
            }

            //DrawModel(cone);
            //DrawModel(juan);

            base.Draw(gameTime);
        }

        // TODO poprawić
        private void DrawModel(Model model)
        {
            //https://docs.microsoft.com/pl-pl/xamarin/graphics-games/monogame/3d/part1
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = Matrix.Identity;

                    var cameraPosition = new Vector3(0, 8, 0);
                    var cameraLookAtVector = Vector3.Zero;
                    var cameraUpVector = Vector3.UnitZ;

                    effect.View = Matrix.CreateLookAt(
                        cameraPosition, cameraLookAtVector, cameraUpVector);

                    float aspectRatio =
                        graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
                    float fieldOfView = MathHelper.PiOver4;
                    float nearClipPlane = 1;
                    float farClipPlane = 200;

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

                }

                mesh.Draw();
            }
        }
    }
}
