using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sounds;


namespace MenuButtons
{
    public class MenuButton
    {
        private SoundEffects ButtonHoverSound, ButtonClickSound;

        private Rectangle Position;
        private Texture2D Sprite, SpriteOnHover;
        private SoundManager SoundManager;

        private bool onHover;

        public Color TintColor { get; set; }

        // -----------------------------------------------------------

        public MenuButton(Texture2D sprite, Texture2D spriteOnHover, Rectangle position, SoundEffect hoverSound)
        {
            BasicAssign();
            Sprite = sprite;
            Position = position;
            SpriteOnHover = spriteOnHover;
            ButtonHoverSound = new SoundEffects(hoverSound);
        }

        public MenuButton(ContentManager Content, string buttonPath, string buttonHoverPath, Point position, string hoverSoundPath, string clickSoundPath)
        {
            BasicAssign();

            Sprite = Content.Load<Texture2D>(buttonPath);
            SpriteOnHover = Content.Load<Texture2D>(buttonHoverPath);

            Position = new Rectangle(position, new Point(Sprite.Width, Sprite.Height));
            ButtonHoverSound = new SoundEffects(Content, hoverSoundPath);
            ButtonClickSound = new SoundEffects(Content, hoverSoundPath);
        }

        public MenuButton(ContentManager Content, string buttonPath, string buttonHoverPath, Point position, SoundManager soundMgr, string hoverSoundName, string clickSoundName)
        {
            BasicAssign();

            Sprite = Content.Load<Texture2D>(buttonPath);
            SpriteOnHover = Content.Load<Texture2D>(buttonHoverPath);

            Position = new Rectangle(position, new Point(Sprite.Width, Sprite.Height));
            SoundManager = soundMgr;

            ButtonHoverSound = new SoundEffects(SoundManager.GetSoundEffect(hoverSoundName));
            ButtonClickSound = new SoundEffects(SoundManager.GetSoundEffect(clickSoundName));
        }
        
        // -----------------------------------------------------------

        public bool IsOn(Point mousePosition)
        {
            var isOn = mousePosition.X >= Position.X && mousePosition.X <= (Position.X + Sprite.Width) &&
                       mousePosition.Y >= Position.Y && mousePosition.Y <= (Position.Y + Sprite.Height);

            if (isOn)
            {
                ButtonHoverSound.Play();
                onHover = true;
            }

            ButtonHoverSound.Wait = isOn;

            return isOn;
        }
        
        public void OnHover(bool hover)
        {
            onHover = hover;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(onHover ? SpriteOnHover : Sprite, Position, TintColor);
        }

        public bool LeftClick(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                ButtonClickSound.Play();
                ButtonClickSound.Wait = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                ButtonClickSound.Wait = false;
            }

            return ButtonClickSound.Wait;
        }


        private void BasicAssign()
        {
            TintColor = Color.White;
            onHover = false;
        }
    }
}