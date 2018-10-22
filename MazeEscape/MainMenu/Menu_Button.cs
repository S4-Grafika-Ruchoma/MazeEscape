﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sounds;


namespace Menu_buttons
{
    public class Menu_Button
    {
        public SoundEffects buttonSound { get; set; }

        Rectangle Position;
        Texture2D Sprite;
        Texture2D SpriteOnClick;
        Color TintColor;
        bool onClick;

        public Menu_Button(Texture2D sprite, Texture2D spriteOnClick, Rectangle position, SoundEffects buttonSound)
        {
            Sprite = sprite;
            Position = position;
            SpriteOnClick = spriteOnClick;
            TintColor = Color.White;
            onClick = false;
            this.buttonSound = buttonSound;
        }

        public bool IsOn(Point mousePosition)
        {
            var position = Position;

            if (mousePosition.X >= position.X && mousePosition.X <= (position.X + Sprite.Width) &&
                mousePosition.Y >= position.Y && mousePosition.Y <= (position.Y + Sprite.Height))
            {

                buttonSound.Play();
                buttonSound.Wait = true;
                return true;
            }
            else
            {
                buttonSound.Wait = false;
            }

            return false;
        }

        public void SetColor(Color color)
        {
            TintColor = color;
        }

        public void OnClick(bool click)
        {
            onClick = click;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (onClick) { spriteBatch.Draw(SpriteOnClick, Position, TintColor); }
            else { spriteBatch.Draw(Sprite, Position, TintColor); }
        }

    }
}