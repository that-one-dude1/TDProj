using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TDProj.Screens
{
    public class GameOverScreen : GameScreen
    {
        private SpriteFont defaultFont;
        private SpriteFont titleFont;
        private Texture2D pixel;
        private GraphicsDevice graphics;

        private MouseState prevMouse;

        private Rectangle exitButtonBounds = new Rectangle(75, 930, 150, 75);

        public event Action OnExit;

        public int waveIndex;

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

            prevMouse = mouse;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, new Vector2(0, 0), new Rectangle(0, 0, 1920, 1080), new Color(32, 0, 32));

            spriteBatch.DrawString(titleFont, "GAME OVER", new Vector2(839.5f, 120), Color.White);

            spriteBatch.DrawString(defaultFont, $"You survived {waveIndex} waves.",
                new Vector2(960 - (defaultFont.MeasureString($"You survived {waveIndex} waves.").X / 2), 240), Color.White);

            spriteBatch.Draw(pixel, new Vector2(75, 930), exitButtonBounds, exitButtonBounds.Contains(Mouse.GetState().Position) ? new Color(30, 30, 30) : Color.White);
            spriteBatch.DrawString(defaultFont, "Exit", new Vector2(120, 950), exitButtonBounds.Contains(Mouse.GetState().Position) ? Color.White : Color.Black);
        }
    }
}
