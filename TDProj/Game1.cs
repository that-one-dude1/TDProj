using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TDProj
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D pixel;
        private Texture2D background;
        private SpriteFont defaultFont;

        private GridManager gridManager;
        private InputManager inputManager;
        private UIManager uiManager;
        private EnemyManager enemyManager;
        private TowerManager towerManager;

        private int screenWidth;
        private int screenHeight;

        private List<Enemy> enemies;
        private List<Point> enemyPath;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            SetBorderlessFullscreen();

            gridManager = new GridManager(32, 18, 60);
            inputManager = new InputManager(gridManager);

            //Generating full path
            List<Point> enemyWaypoints = new()
            {
                new Point(0, 5),
                new Point(5, 5),
                new Point(5, 10),
                new Point(20, 10),
                new Point(20, 5),
                new Point(31, 5)
            };

            enemyPath = PathGenerator.GenerateFullPath(enemyWaypoints);
            gridManager.SetPath(enemyPath);
            enemies = new List<Enemy>();

            base.Initialize();
        }

        private void SetBorderlessFullscreen()
        {
            graphics.HardwareModeSwitch = false;
            graphics.IsFullScreen = false;
            Window.IsBorderless = true;

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            Window.Position = new Point(0, 0);
        }

        private Texture2D CreateCircleTexture(int radius)
        {
            int diameter = radius * 2;
            Texture2D texture = new Texture2D(GraphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];

            float radiusSq = radius * radius;

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    int index = y * diameter + x;
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    colorData[index] = pos.LengthSquared() <= radiusSq ? Color.White : Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            uiManager = new UIManager(new Rectangle(0, 720, 1920, 360), pixel, defaultFont);

            background = Content.Load<Texture2D>("background");
            defaultFont = Content.Load<SpriteFont>("defaultFont");

            var circle= CreateCircleTexture(30);
            var triangle = Content.Load<Texture2D>("triangle");
            var pentagon = Content.Load<Texture2D>("pentagon");

            uiManager = new UIManager(new Rectangle(0, 720, 1920, 360), pixel, defaultFont);
            enemyManager = new EnemyManager(enemyPath, circle, triangle, pentagon, gridManager.CellSize);
            towerManager = new TowerManager(gridManager, pixel);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            inputManager.Update(Mouse.GetState());

            uiManager.HandleInput(inputManager.MousePosition, inputManager.LeftClicked);

            if (inputManager.LeftClicked)
            {
                if (uiManager.Visible && !uiManager.IsPointInUI(inputManager.MousePosition))
                    uiManager.Hide();
                else if (inputManager.MouseInsideGrid)
                    uiManager.Show(inputManager.HoveredCell);
            }

            if (inputManager.LeftClicked && !uiManager.IsPointInUI(inputManager.MousePosition))
            {
                towerManager.TryPlaceTower(inputManager.HoveredCell, uiManager.SelectedTowerType);
                uiManager.selectedTowerType = null;
            }

            enemyManager.Update(gameTime);
            towerManager.Update(gameTime, enemyManager.GetEnemies());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            towerManager.Draw(spriteBatch);

            gridManager.Draw(spriteBatch, pixel, inputManager.HoveredCell, inputManager.MouseInsideGrid);
            uiManager.Draw(spriteBatch);

            enemyManager.Draw(spriteBatch);
            enemyManager.DrawUI(spriteBatch, defaultFont);

            spriteBatch.End();
            base.Draw(gameTime);
        }

            
    }
}
