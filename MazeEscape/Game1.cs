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


            base.Draw(gameTime);
        }
    }
}
