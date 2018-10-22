using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sounds;


namespace Menu_buttons
{
    public class Menu_Button
    {
        public SoundEffects buttonSound { get; set; }

        private Rectangle Position;
        private Texture2D Sprite;
        private Texture2D SpriteOnClick;
        private Color TintColor;
        private bool onHover;

        public Menu_Button(Texture2D sprite, Texture2D spriteOnClick, Rectangle position, SoundEffect buttonSound)
        {
            Sprite = sprite;
            Position = position;
            SpriteOnClick = spriteOnClick;
            TintColor = Color.White;
            onHover = false;
            this.buttonSound = new SoundEffects(buttonSound);
        }

        public bool IsOn(Point mousePosition)
        {
            var isOn = mousePosition.X >= Position.X && mousePosition.X <= (Position.X + Sprite.Width) &&
                       mousePosition.Y >= Position.Y && mousePosition.Y <= (Position.Y + Sprite.Height);

            if (isOn)
            {
                buttonSound.Play();
            }

            buttonSound.Wait = isOn;

            return isOn;
        }


        public void OnHover(bool hover)
        {
            onHover = hover;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(onHover ? SpriteOnClick : Sprite, Position, TintColor);
        }

    }
}