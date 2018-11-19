﻿using System.Collections.Generic;
using System.Linq;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeEscape.MainMenu
{
    public class MainMenu
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D menuBackground;
        SoundEffects button_click;
        private MouseState prevState;
        SpriteFont Font;

        public MenuState State;

        bool mouseLock = false;
        bool opcjePressed = false;
        bool autorzyPressed = false;

        List<MenuButton> Buttons;
        private SoundManager soundMgr;
        private Game1 Game;

        public MainMenu(Game1 game, SoundManager soundMgr, GraphicsDeviceManager graphicsDevice)
        {
            Game = game;
            graphics = graphicsDevice;
            State = MenuState.MainMenu;
            this.soundMgr = soundMgr;
            LoadContent();
        }

        protected void LoadContent()
        {
            Font = Game.Content.Load<SpriteFont>("Fonts/SpriteFontPL");
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            menuBackground = Game.Content.Load<Texture2D>("Main_Menu/Menu_background");

            soundMgr.Play("menu-ambient", 0.5f, true);

            int xOffset = 130, yOffset = 400, yPadding = 65;

            Buttons = new List<MenuButton>
            {
                new MenuButton(Game.Content, "Main_Menu/Graj_buttnon_A", "Main_Menu/Graj_buttnon_B",
                    new Point(xOffset, yOffset), soundMgr, "menu-btn-hover", "menu-btn-click", MenuState.MainMenu),

                new MenuButton(Game.Content, "Main_Menu/Autorzy_buttnon_A", "Main_Menu/Autorzy_buttnon_B",
                    new Point(xOffset, yOffset + yPadding), soundMgr, "menu-btn-hover", "menu-btn-click",MenuState.MainMenu),

                new MenuButton(Game.Content, "Main_Menu/Opcje_buttnon_A", "Main_Menu/Opcje_buttnon_B",
                    new Point(xOffset, yOffset + yPadding * 2), soundMgr, "menu-btn-hover", "menu-btn-click",MenuState.MainMenu),

                new MenuButton(Game.Content, "Main_Menu/Wyjscie_buttnon_A", "Main_Menu/Wyjscie_buttnon_B",
                    new Point(xOffset, yOffset + yPadding * 3), soundMgr, "menu-btn-hover", "menu-btn-click",MenuState.MainMenu),

                new MenuButton(Game.Content, "Main_Menu/Back_buttnon_A", "Main_Menu/Back_buttnon_B",
                    new Point(50, Game.GraphicsDevice.Viewport.Height-200), soundMgr, "menu-btn-hover", "menu-btn-click",MenuState.Authors), // TODO Przycisk powrotu

                new MenuButton(Game.Content, "Main_Menu/Back_buttnon_A", "Main_Menu/Back_buttnon_B",
                    new Point(50, Game.GraphicsDevice.Viewport.Height-200), soundMgr, "menu-btn-hover", "menu-btn-click",MenuState.Options), // TODO Przycisk powrotu
            };
            // i = 0  PLAY button
            // i = 3  EXIT Button
        }

        public void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var mousePos = new Point(mouseState.X, mouseState.Y);

            if (State == MenuState.MainMenu && prevState.LeftButton == ButtonState.Released)
            {
                // Rozpocznij gre
                if (Buttons[0].IsOn(mousePos) && Buttons[0].LeftClick(mouseState) && !mouseLock)
                {
                    Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
                    Game.RunGame = false;
                    soundMgr.Stop("menu-ambient");

                    if (State == MenuState.MainMenu)
                    {
                        // Odtworzenie dzwięków
                        //soundMgr.Play("talk-1",0.1f);
                        soundMgr.Play("game-ambient", 0.7f);
                    }
                    else
                    {
                        soundMgr.Resume("game-ambient");
                        //soundMgr.Play("talk-1",0.1f);
                    }
                }
                else if (Buttons[1].IsOn(mousePos) && Buttons[1].LeftClick(mouseState) && !mouseLock)
                {
                    State = MenuState.Authors;
                }
                else if (Buttons[2].IsOn(mousePos) && Buttons[2].LeftClick(mouseState) && !mouseLock)
                {
                    State = MenuState.Options;
                }
                else if (Buttons[3].IsOn(mousePos) && Buttons[3].LeftClick(mouseState) && !mouseLock)
                {
                    soundMgr.Stop("menu-ambient");
                    Game.Exit();
                }
            }
            else if (State == MenuState.Authors && prevState.LeftButton == ButtonState.Released)
            {
                if (Buttons[4].IsOn(mousePos) && Buttons[4].LeftClick(mouseState) && !mouseLock)
                {
                    State = MenuState.MainMenu;
                }
            }
            else if (State == MenuState.Options && prevState.LeftButton == ButtonState.Released)
            {
                if (Buttons[4].IsOn(mousePos) && Buttons[4].LeftClick(mouseState) && !mouseLock)
                {
                    State = MenuState.MainMenu;
                }
                else if (soundToggle.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released && !mouseLock)
                {
                    AppConfig.PLAY_SOUNDS = !AppConfig.PLAY_SOUNDS;
                    if (AppConfig.PLAY_SOUNDS)
                    {
                        soundMgr.Play("menu-ambient", 0.5f, true);
                    }
                    else
                    {
                        soundMgr.Stop("menu-ambient");
                    }
                }
            }

            foreach (var button in Buttons.Where(a => a.BelongsToState == State))
            {
                button.OnHover(button.IsOn(mousePos));
            }

            prevState = mouseState;
        }

        Rectangle soundToggle = new Rectangle(400, 350, 200, 50);

        public void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);

            foreach (var menuButton in Buttons.Where(a => a.BelongsToState == State))
            {
                menuButton.Draw(spriteBatch);
            }

            if (State == MenuState.Options)
            {
                spriteBatch.DrawString(Font, $"Włącz dzwięk", new Vector2(400, 350), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                if (AppConfig.PLAY_SOUNDS)
                {
                    spriteBatch.DrawString(Font, $"TAK", new Vector2(550, 340), Color.Green, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.DrawString(Font, $"Nie", new Vector2(550, 340), Color.Green, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);
                }

                spriteBatch.DrawString(Font, $"Zmiana czułości myszy", new Vector2(400, 500), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);

                spriteBatch.DrawString(Font, $"[-]", new Vector2(600, 490), Color.Green, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"{AppConfig._MOUSE_SPEED}", new Vector2(650, 490), Color.Green, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"[+]", new Vector2(750, 490), Color.Green, 0, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0);

            }
            else if (State == MenuState.Authors)
            {
                spriteBatch.DrawString(Font, $"Autorzy", new Vector2(500, 150), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"Sekcja 4 2018 MazeEscape", new Vector2(600, 205), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"Alan Biegun - kolizje, inerakcje, wyświetlanie modeli 3D", new Vector2(500, 300), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"Albert Francik - generacja lowowych labiryntów", new Vector2(500, 350), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"Dawid Ryl - przeciwniki i sztuczna inteligencja", new Vector2(500, 400), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"Jan Snopek - modele, animacje i menu", new Vector2(500, 450), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Font, $"Dominik Żurawski - tworzenie dynamicznego oświetlenia", new Vector2(500, 500), Color.White, 0, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }
    }
}

