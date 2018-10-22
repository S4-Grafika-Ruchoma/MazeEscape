using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeEscape;
using Menu_buttons;
using Sounds;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MainMenu
{
    public class MainMenu : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public bool runGame { get; set; }
        Texture2D menuBackground, bG_A, bG_B, bA_A, bA_B, bO_A, bO_B, bW_A, bW_B;
        bool mouseLock = false;
        SoundEffects button_click;

        private BackgroundSongs bgSong;

        List<Menu_Button> knefel;

        public MainMenu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            runGame = false;
            knefel = new List<Menu_Button>();


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuBackground = Content.Load<Texture2D>("Main_Menu/Menu_background");

            // var button_click1 = Content.Load<SoundEffect>("Sounds/menu_click");

            bgSong = new BackgroundSongs(Content.Load<Song>("Music/menu"),true,50f);
            bgSong.Play();

            var hoverSound = Content.Load<SoundEffect>("Sounds/menu_click");

            // Buttons Load

            bG_A = Content.Load<Texture2D>("Main_Menu/Graj_buttnon_A");
            bG_B = Content.Load<Texture2D>("Main_Menu/Graj_buttnon_B");

            bA_A = Content.Load<Texture2D>("Main_Menu/Autorzy_buttnon_A");
            bA_B = Content.Load<Texture2D>("Main_Menu/Autorzy_buttnon_B");

            bO_A = Content.Load<Texture2D>("Main_Menu/Opcje_buttnon_A");
            bO_B = Content.Load<Texture2D>("Main_Menu/Opcje_buttnon_B");

            bW_A = Content.Load<Texture2D>("Main_Menu/Wyjscie_buttnon_A");
            bW_B = Content.Load<Texture2D>("Main_Menu/Wyjscie_buttnon_B");

            int xOffset = 130;
            int yOffset = 400;
            int yPadding = 65;

            knefel.Add(new Menu_Button(bG_A, bG_B, new Rectangle(xOffset, yOffset, bG_A.Width, bG_B.Height), hoverSound)); // i = 0  PLAY button
            knefel.Add(new Menu_Button(bA_A, bA_B, new Rectangle(xOffset, yOffset + yPadding, bG_A.Width, bG_B.Height), hoverSound));
            knefel.Add(new Menu_Button(bO_A, bO_B, new Rectangle(xOffset, yOffset + yPadding * 2, bG_A.Width, bG_B.Height), hoverSound));
            knefel.Add(new Menu_Button(bW_A, bW_B, new Rectangle(xOffset, yOffset + yPadding * 3, bG_A.Width, bG_B.Height), hoverSound)); // i = 3  EXIT Button
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

            if (knefel[0].IsOn(mousePos) && mouseState.LeftButton == ButtonState.Pressed && !mouseLock)
            {
                runGame = true;
                Exit();
            }

            if (knefel[3].IsOn(mousePos) && mouseState.LeftButton == ButtonState.Pressed && !mouseLock)
            {
                Exit();
            }


            for (int i = 0; i < knefel.Count; i++)
            {
                if (knefel[i].IsOn(mousePos))
                {

                    knefel[i].OnHover(true);
                }
                else
                {
                    knefel[i].OnHover(false);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            for (int i = 0; i < 4; i++)
            {
                knefel[i].Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

