using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TDProj.Screens
{
    public class StartMenuScreen : GameScreen
    {
        private int selectedIndex = 0;

        private SpriteFont defaultFont;
        private SpriteFont titleFont;
        private Texture2D background;
        private Texture2D pixel;
        private GraphicsDevice graphics;
        private KeyboardState prevKeyboard;
        private MouseState prevMouse;

        private Dictionary<MenuButtons, StartMenuButtons> Buttons;
        private MenuButtons[] menuButtons = { MenuButtons.StartGame, MenuButtons.LoadSave, MenuButtons.Instructions, MenuButtons.Exit };

        private int screenWidth;
        private int screenHeight;

        public event Action OnStartGame;
        public event Action OnLoadSave;
        public event Action OnInstructionsMenu;
        public event Action OnExit;

        public override void LoadContent(GraphicsDevice graphicsDevice, SpriteFont font, SpriteFont titleFont, Texture2D background, Texture2D pixel)
        {
            this.graphics = graphicsDevice;
            this.background = background;
            this.pixel = pixel;
            this.defaultFont = font;
            this.titleFont = titleFont;
            this.Buttons = StartMenuButtons.GetMenuButtons();
        }

        public override void Update(GameTime gameTime)
        {

            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            StartMenuButtons button = Buttons[menuButtons[selectedIndex]];

            if (IsKeyPressed(Keys.Down, kb))
                selectedIndex = (selectedIndex + 1) % menuButtons.Length;
            else if (IsKeyPressed(Keys.Up, kb))
                selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
            else if (IsKeyPressed(Keys.Enter, kb))
                HandleSelection();

            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            selectedIndex = GetHoveredItemIndex(mousePos);

            Rectangle bounds = button.Bounds;

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && bounds.Contains(mousePos))
            {
                if (selectedIndex >= 0)
                    HandleSelection();
            }

            prevKeyboard = kb;
            prevMouse = mouse;
        }

        private bool IsKeyPressed(Keys key, KeyboardState kb)
        {
            return kb.IsKeyDown(key) && prevKeyboard.IsKeyUp(key);
        }

        private void HandleSelection()
        {
            switch (menuButtons[selectedIndex])
            {
                case MenuButtons.StartGame:
                    OnStartGame?.Invoke();
                    break;
                case MenuButtons.LoadSave:
                    OnLoadSave?.Invoke();
                    break;
                case MenuButtons.Instructions:
                    OnInstructionsMenu?.Invoke();
                    break;
                case MenuButtons.Exit:
                    OnExit?.Invoke();
                    break;
                default:
                    break;
            }
        }

        private int GetHoveredItemIndex(Vector2 mousePos)
        {
            int screenWidth = graphics.Viewport.Width;
            for (int i = 0; i < menuButtons.Length; i++)
            {
                StartMenuButtons button = Buttons[menuButtons[i]];

                if (button.Bounds.Contains(mousePos))
                    return i;
            }

            return selectedIndex; //no change if not hovering anything
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            string title = "MonoGame Tower Defence";
            Vector2 titleSize = titleFont.MeasureString(title);
            spriteBatch.DrawString(titleFont, title, new Vector2(screenWidth / 2 - titleSize.X / 2, 225), Color.Black);

            for (int i = 0; i < menuButtons.Length; i++)
            {
                Color color = Color.White;

                StartMenuButtons button = Buttons[menuButtons[i]];

                spriteBatch.Draw(pixel, button.Position, button.Bounds, (i == selectedIndex) ? Color.DarkGreen : Color.Green);
                spriteBatch.DrawString(defaultFont, button.Text, new Vector2(button.Position.X + 20, button.Position.Y + 20), color);
            }
        }
    }
}
