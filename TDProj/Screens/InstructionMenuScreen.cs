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
    public class InstructionMenuScreen : GameScreen
    {
        private SpriteFont defaultFont;
        private SpriteFont titleFont;
        private Texture2D background;
        private Texture2D pixel;
        private GraphicsDevice graphics;

        private int screenWidth;
        private int screenHeight;

        private KeyboardState prevKeyboard;
        private MouseState prevMouse;

        private Rectangle exitButtonBounds = new Rectangle(1695, 930, 150, 75);

        public event Action OnExit;

        public override void LoadContent(GraphicsDevice graphicsDevice, SpriteFont font, SpriteFont titleFont, Texture2D background, Texture2D pixel)
        {
            this.graphics = graphicsDevice;
            this.pixel = pixel;
            this.defaultFont = font;
            this.titleFont = titleFont;
            this.background = background;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && exitButtonBounds.Contains(mouse.Position))
                HandleExitSelection();
            if (IsKeyPressed(Keys.Back, kb))
                HandleExitSelection();

            prevKeyboard = kb;
            prevMouse = mouse;
        }

        private bool IsKeyPressed(Keys key, KeyboardState kb)
        {
            return kb.IsKeyDown(key) && prevKeyboard.IsKeyUp(key);
        }

        private void HandleExitSelection()
        {
            OnExit?.Invoke();
        }

        private float CentreText(string text, SpriteFont font)
        {
            Vector2 textLength = font.MeasureString(text);
            float output = (screenWidth / 2) - (textLength.X / 2);
            return output;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            string text1 = "Place a tower in a grid square to begin attacking the shapes.";
            string text2 = "Placing and upgrading towers requires money, earn money through defeating shapes and clearing waves.";
            string text3 = "Press \"U\" on your keyboard to close and open the placing menu.";
            string text4 = "Don't let the shapes reach the end of the track, as you will take damage.";

            spriteBatch.DrawString(titleFont, "Instructions:", new Vector2 (CentreText("Instructions:", titleFont), 100), Color.White);

            spriteBatch.DrawString(defaultFont, text1, new Vector2(CentreText(text1, defaultFont), 300), Color.White);
            spriteBatch.DrawString(defaultFont, text2, new Vector2(CentreText(text2, defaultFont), 400), Color.White);
            spriteBatch.DrawString(defaultFont, text3, new Vector2(CentreText(text3, defaultFont), 500), Color.White);
            spriteBatch.DrawString(defaultFont, text4, new Vector2(CentreText(text4, defaultFont), 600), Color.White);

            spriteBatch.Draw(pixel, new Vector2(1695, 930), exitButtonBounds, exitButtonBounds.Contains(Mouse.GetState().Position)? Color.DarkSlateGray : Color.White);
            spriteBatch.DrawString(defaultFont, "Exit", new Vector2(1740, 950), exitButtonBounds.Contains(Mouse.GetState().Position)? Color.White : Color.Black);
        }
    }
}
