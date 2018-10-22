using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjektTestowy.CustomClasses;

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
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(10, 1, 5), Vector3.Zero, 5);
            Components.Add(camera);

            floor = new Floor(GraphicsDevice, 20, 20);

            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1,
                VertexColorEnabled = true,
                LightingEnabled = false,
            };

            base.Initialize();
        }

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

            base.Draw(gameTime);
        }
    }
}
