using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TDProj.Screens
{
    public abstract class GameScreen
    {
        public abstract void LoadContent(GraphicsDevice graphicsDevice, SpriteFont font, SpriteFont titleFont, Texture2D background, Texture2D pixel);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }

    public class ScreenManager()
    {
        private GameScreen currentScreen;

        public void ChangeScreen(GameScreen newScreen, GraphicsDevice graphicsDevice, SpriteFont font, SpriteFont titleFont, Texture2D background, Texture2D pixel)
        {
            currentScreen = newScreen;
            currentScreen.LoadContent(graphicsDevice, font, titleFont, background, pixel);
        }

        public void Update(GameTime gameTime)
        {
            currentScreen?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen?.Draw(spriteBatch);
        }
    }
}
