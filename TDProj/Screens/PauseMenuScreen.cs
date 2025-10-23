using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDProj.Screens
{
    public class PauseMenuScreen : GameScreen
    {
        private SpriteFont defaultFont;
        private SpriteFont titleFont;
        private Texture2D pixel;
        private GraphicsDevice graphics;

        private MouseState prevMouse;

        private Rectangle resumeButtonBounds = new Rectangle(1695, 930, 150, 75);
        private Rectangle exitButtonBounds = new Rectangle(75, 930, 150, 75);

        public event Action OnExit;
        public event Action OnResume;

        public override void LoadContent(GraphicsDevice graphicsDevice, SpriteFont font, SpriteFont titleFont, Texture2D background, Texture2D pixel)
        {
            this.graphics = graphicsDevice;
            this.defaultFont = font;
            this.titleFont = titleFont;
            this.pixel = pixel;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && exitButtonBounds.Contains(mouse.Position))
                OnExit?.Invoke();

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && resumeButtonBounds.Contains(mouse.Position))
                OnResume?.Invoke();

            prevMouse = mouse;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, new Vector2(0, 0), new Rectangle(0, 0, 1920, 1080), Color.Black * 0.6f);

            spriteBatch.Draw(pixel, new Vector2(1695, 930), resumeButtonBounds, resumeButtonBounds.Contains(Mouse.GetState().Position) ? Color.DarkSlateGray : Color.White);
            spriteBatch.DrawString(defaultFont, "Resume", new Vector2(1720, 950), resumeButtonBounds.Contains(Mouse.GetState().Position) ? Color.White : Color.Black);

            spriteBatch.Draw(pixel, new Vector2(75, 930), exitButtonBounds, exitButtonBounds.Contains(Mouse.GetState().Position) ? Color.DarkSlateGray : Color.White);
            spriteBatch.DrawString(defaultFont, "Exit", new Vector2(120, 950), exitButtonBounds.Contains(Mouse.GetState().Position) ? Color.White : Color.Black);
        }
    }
}
