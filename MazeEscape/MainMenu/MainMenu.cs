using System.Collections.Generic;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeEscape.MainMenu
{
    public class MainMenu : Game
    {
        public bool runGame { get; set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D menuBackground;
        SoundEffects button_click;

        bool mouseLock = false;
        bool opcjePressed = false;
        bool autorzyPressed = false;

        private SoundManager soundMgr;

        List<MenuButton> Buttons;

        public MainMenu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = AppConfig.HEIGHT;
            graphics.PreferredBackBufferWidth = AppConfig.WIDTH;
            graphics.IsFullScreen = AppConfig.FULL_SCREEN;
            IsMouseVisible = AppConfig.IS_MOUSE_VISIBLE;
        }

        protected override void Initialize()
        {
            runGame = false;
            Buttons = new List<MenuButton>();
            soundMgr = new SoundManager(Content);

            base.Initialize();

            if (AppConfig._DEBUG_SKIP_MAIN_MENU_)
            {
                runGame = true;
                Exit();
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuBackground = Content.Load<Texture2D>("Main_Menu/Menu_background");

            soundMgr.Add(
                new Dictionary<string, string>()
                {
                    {"menu-ambient","Music/menu" }
                },
                new Dictionary<string, string>()
                {
                    {"menu-btn-hover","Sounds/menu_click"},
                    {"menu-btn-click","Sounds/lose sound 1_0"},
                }
                );

            soundMgr.Play("menu-ambient",true);

            int xOffset = 130, yOffset = 400, yPadding = 65;

            Buttons.Add(new MenuButton(Content, "Main_Menu/Graj_buttnon_A", "Main_Menu/Graj_buttnon_B", new Point(xOffset, yOffset), soundMgr, "menu-btn-hover", "menu-btn-click")); // i = 0  PLAY button
            Buttons.Add(new MenuButton(Content, "Main_Menu/Autorzy_buttnon_A", "Main_Menu/Autorzy_buttnon_B", new Point(xOffset, yOffset + yPadding), soundMgr, "menu-btn-hover", "menu-btn-click"));
            Buttons.Add(new MenuButton(Content, "Main_Menu/Opcje_buttnon_A", "Main_Menu/Opcje_buttnon_B", new Point(xOffset, yOffset + yPadding * 2), soundMgr, "menu-btn-hover", "menu-btn-click"));
            Buttons.Add(new MenuButton(Content, "Main_Menu/Wyjscie_buttnon_A", "Main_Menu/Wyjscie_buttnon_B", new Point(xOffset, yOffset + yPadding * 3), soundMgr, "menu-btn-hover", "menu-btn-click")); // i = 3  EXIT Button
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var mousePos = new Point(mouseState.X, mouseState.Y);
            var keyboardState = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                runGame = true;
                Exit();
            }

            // Rozpocznij gre
            if (Buttons[0].IsOn(mousePos) && Buttons[0].LeftClick(mouseState) && !mouseLock &&!opcjePressed && !autorzyPressed)
            {
                runGame = true;
                soundMgr.Stop("menu-ambient");
                Exit();
            }

            // Autorzy 
            if (Buttons[1].IsOn(mousePos) && Buttons[1].LeftClick(mouseState) && !mouseLock && !opcjePressed && !autorzyPressed)
            {
                autorzyPressed = true;
            }

            // Opcje
            if (Buttons[2].IsOn(mousePos) && Buttons[2].LeftClick(mouseState) && !mouseLock && !opcjePressed && !autorzyPressed)
            {
                opcjePressed = true;
            }

            // Wyjdź
            if (Buttons[3].IsOn(mousePos) && Buttons[3].LeftClick(mouseState) && !mouseLock && !opcjePressed && !autorzyPressed)
            {
                soundMgr.Stop("menu-ambient");
                Exit();
            }


            foreach (var button in Buttons)
            {
                if (button.IsOn(mousePos))
                {
                    button.OnHover(button.IsOn(mousePos));
                }
                else
                {
                    button.OnHover(false);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);


            if(opcjePressed)
            {

            }

            else if(autorzyPressed)
            {

            }

            else
            {
                // Rysowanie MainMenu
                for (int i = 0; i < 4; i++)
                {
                    Buttons[i].Draw(spriteBatch);
                }
            }
           
            



            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

