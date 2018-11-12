using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeEscape.MainMenu
{
    public class MenuButton
    {
        private SoundEffects ButtonHoverSound, ButtonClickSound;

        public MenuState BelongsToState { get; set; }

        private Rectangle Position;
        private Texture2D Sprite, SpriteOnHover;
        private SoundManager SoundManager;

        private bool onHover;

        public Color TintColor { get; set; }

        // -----------------------------------------------------------


        public MenuButton(ContentManager Content, string buttonPath, string buttonHoverPath, Point position, SoundManager soundMgr, string hoverSoundName, string clickSoundName, MenuState state)
        {
            BasicAssign();

            Sprite = Content.Load<Texture2D>(buttonPath);
            SpriteOnHover = Content.Load<Texture2D>(buttonHoverPath);

            Position = new Rectangle(position, new Point(Sprite.Width, Sprite.Height));
            SoundManager = soundMgr;

            ButtonHoverSound = new SoundEffects(SoundManager.GetSoundEffect(hoverSoundName));
            ButtonClickSound = new SoundEffects(SoundManager.GetSoundEffect(clickSoundName));

            BelongsToState = state;
        }
        
        // -----------------------------------------------------------

        public bool IsOn(Point mousePosition)
        {
            var isOn = mousePosition.X >= Position.X && mousePosition.X <= (Position.X + Sprite.Width) &&
                       mousePosition.Y >= Position.Y && mousePosition.Y <= (Position.Y + Sprite.Height);

            if (isOn)
            {
                ButtonHoverSound.Play(0.5f);
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
                ButtonClickSound.Play(0.5f);
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