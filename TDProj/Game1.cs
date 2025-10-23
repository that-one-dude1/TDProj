using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDProj.Screens;

namespace TDProj
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        enum GameState
        {
            StartMenu,
            LoadSaveMenu,
            InstructionsMenu,
            Playing, 
            Paused,
            GameOver
        }

        private GameState currentState = GameState.StartMenu;

        private Texture2D pixel;
        private Texture2D background;
        private Texture2D menuBackground;
        private Texture2D instructionsBackground;
        private SpriteFont defaultFont;
        private SpriteFont titleFont;
        private SpriteFont smallFont;

        private GridManager gridManager;
        private InputManager inputManager;
        private UIManager uiManager;
        private EnemyManager enemyManager;
        private TowerManager towerManager;
        private PlayerResources playerResources;
        private ScreenManager screenManager;
        private InstructionMenuScreen instructionMenu;
        private StartMenuScreen startMenu;
        private PauseMenuScreen pauseMenu;
        private GameOverScreen gameOverScreen;
        private SaveManager saveManager;
        private AchievementManager achievementManager;
        private EnemySpawner enemySpawner;

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
            screenManager = new ScreenManager();
            saveManager = new SaveManager();

            //Generating full path
            List<Point> enemyWaypoints = new()
            {
                new Point(0, 4),
                new Point(6, 4),
                new Point(6, 9),
                new Point(16, 9),
                new Point(16, 4),
                new Point(13, 4),
                new Point(13, 7),
                new Point(23, 7),
                new Point(23, 12),
                new Point(28, 12),
                new Point(28, 9),
                new Point(31, 9),
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

            playerResources = new PlayerResources(500);

            background = Content.Load<Texture2D>("plains-background");
            menuBackground = Content.Load<Texture2D>("menu-background");
            instructionsBackground = Content.Load<Texture2D>("instructions-background");
            defaultFont = Content.Load<SpriteFont>("defaultFont");
            titleFont = Content.Load<SpriteFont>("titleFont");
            smallFont = Content.Load<SpriteFont>("smallFont");

            startMenu = new StartMenuScreen();
            startMenu.OnStartGame += () => currentState = GameState.Playing;
            startMenu.OnLoadSave += () => currentState = GameState.LoadSaveMenu;
            startMenu.OnInstructionsMenu += () => currentState = GameState.InstructionsMenu;
            startMenu.OnExit += SaveAndExit;
            startMenu.LoadContent(GraphicsDevice, defaultFont, titleFont, menuBackground, pixel);

            instructionMenu = new InstructionMenuScreen();
            instructionMenu.OnExit += () => currentState = GameState.StartMenu;
            instructionMenu.LoadContent(GraphicsDevice, defaultFont, titleFont, instructionsBackground, pixel);

            pauseMenu = new PauseMenuScreen();
            pauseMenu.OnExit += () => currentState = GameState.StartMenu;
            pauseMenu.OnResume += () => currentState = GameState.Playing;
            pauseMenu.LoadContent(GraphicsDevice, defaultFont, titleFont, null, pixel);

            gameOverScreen = new GameOverScreen();
            gameOverScreen.OnExit += () => currentState = GameState.StartMenu;
            gameOverScreen.LoadContent(GraphicsDevice, defaultFont, titleFont, null, pixel);

            switch(currentState)
            {
                case GameState.InstructionsMenu:
                    screenManager.ChangeScreen(instructionMenu, GraphicsDevice, defaultFont, titleFont, instructionsBackground, pixel);
                    break;
                case GameState.StartMenu:
                    screenManager.ChangeScreen(startMenu, GraphicsDevice, defaultFont, titleFont, menuBackground, pixel);
                    break;
                case GameState.GameOver:
                    screenManager.ChangeScreen(gameOverScreen, GraphicsDevice, defaultFont, titleFont, null, pixel);
                    break;
            }
            
            var circle = CreateCircleTexture(30);
            var rangeTexture = CreateCircleTexture(100); //base range circle (scaled per tower)
            var triangle = Content.Load<Texture2D>("triangle");
            var pentagon = Content.Load<Texture2D>("pentagon");
            var hexagon = Content.Load<Texture2D>("hexagon");

            towerManager = new TowerManager(gridManager, pixel, playerResources, rangeTexture);
            uiManager = new UIManager(new Rectangle(0, 720, 1920, 360), pixel, defaultFont, playerResources, towerManager);
            enemyManager = new EnemyManager(enemyPath, circle, triangle, pentagon, hexagon, uiManager, gridManager.CellSize, playerResources);
            enemySpawner = new EnemySpawner(enemyManager);
            enemyManager.spawner = enemySpawner;
            towerManager.uiManager = uiManager;
            achievementManager = new AchievementManager(pixel, defaultFont, smallFont, playerResources, enemyManager, towerManager);

            string json = File.ReadAllText("../../../Enemies/waveList.txt");
            enemyManager.waveScript = JsonSerializer.Deserialize<List<string>>(json);
        }

        private void SaveAndExit()
        {
            saveManager.SaveFile(enemyManager.currentWaveIndex, playerResources.Lives, playerResources.Money, towerManager.towers, towerManager.occupiedCells);
            Exit();
        }

        protected override void Update(GameTime gameTime)
        {

            switch (currentState)
            {
                case GameState.StartMenu:

                    inputManager.Update(Mouse.GetState(), Keyboard.GetState());

                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || inputManager.ExitPressed)
                        Exit();

                    startMenu.Update(gameTime);
                    break;

                case GameState.LoadSaveMenu:

                    SaveData data = SaveManager.LoadFile();

                    if (data != null)
                    {
                        towerManager.Reset();
                        enemyManager.Reset(true);
                        playerResources.Reset();

                        towerManager.towers = data.Towers
                                                .Select(t => TowerManager.FromData(t, pixel))
                                                .ToList();

                        towerManager.occupiedCells = new HashSet<Point>(data.OccupiedCells);

                        enemyManager.currentWaveIndex = data.CurrentWaveIndex;

                        playerResources.Money = data.PlayerMoney;
                        playerResources.Lives = data.PlayerLives;
                    }

                    currentState = GameState.StartMenu;
                    break;

                case GameState.InstructionsMenu:

                    instructionMenu.Update(gameTime);
                    break;


                case GameState.Playing:

                    inputManager.Update(Mouse.GetState(), Keyboard.GetState());

                    if (inputManager.ExitPressed)
                        currentState = GameState.Paused;

                    //if (inputManager.CurrentKeyboard.IsKeyDown(Keys.K))
                    //    playerResources.Lives = 0;

                    if (playerResources.Lives == 0)
                        HandleGameOver();

                    uiManager.HandleInput(inputManager.MousePosition, inputManager.LeftClicked, inputManager.HoveredCell);

                    if (uiManager.selectedTowerType != null && !uiManager.IsPointInUI(inputManager.MousePosition))
                    {
                        uiManager.Hide();
                        uiManager.placeMode = true;
                    }
                    else if (!uiManager.placeMode && uiManager.selectedTowerType == null && towerManager.SelectedTower == null)
                    {
                        uiManager.Show();
                    }

                    if (inputManager.LeftClicked)
                    {
                        Point cell = inputManager.HoveredCell;
                        //Debug.WriteLine(cell);

                        //If clicking on a tower, select it
                        if ((uiManager.upgradeMode && towerManager.SelectedTower == null) || !uiManager.IsPointInUI(inputManager.MousePosition))
                            towerManager.HandleTowerClick(inputManager.MousePosition);

                        //If no tower selected, try to place one
                        if (towerManager.SelectedTower == null && uiManager.placeMode)
                        {
                            towerManager.TryPlaceTower(cell, uiManager.GetSelectedTowerType());
                            uiManager.selectedTowerType = null; //Deselect after placing
                            uiManager.placeMode = false;
                            uiManager.Show(); //reopen UI
                        }
                    }

                    if (inputManager.UpgradePressed && uiManager.upgradeMode)
                        uiManager.upgradeMode = false;
                    else if (inputManager.UpgradePressed && towerManager.SelectedTower == null)
                        uiManager.upgradeMode = true;

                    enemySpawner.Update(gameTime);
                    enemyManager.Update(gameTime);
                    towerManager.Update(gameTime, enemyManager.GetEnemies());
                    base.Update(gameTime);
                    break;

                case GameState.Paused:

                    inputManager.Update(Mouse.GetState(), Keyboard.GetState());

                    if (inputManager.ExitPressed)
                        currentState = GameState.Playing;

                    pauseMenu.Update(gameTime);

                    break;

                case GameState.GameOver:

                    inputManager.Update(Mouse.GetState(), Keyboard.GetState());

                    if (inputManager.ExitPressed)
                        currentState = GameState.StartMenu;

                    gameOverScreen.Update(gameTime);

                    break;
            }
        }

        private void HandleGameOver()
        {
            currentState = GameState.GameOver;
            gameOverScreen.waveIndex = enemyManager.currentWaveIndex;

            playerResources.Reset();
            towerManager.Reset();
            enemyManager.Reset();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            switch (currentState)
            {
                case GameState.StartMenu:
                    startMenu.Draw(spriteBatch);
                    break;

                case GameState.InstructionsMenu:
                    instructionMenu.Draw(spriteBatch);
                    break;

                case GameState.Playing:
                    screenManager.Draw(spriteBatch);

                    spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    gridManager.Draw(spriteBatch, pixel, inputManager.HoveredCell, inputManager.MouseInsideGrid, isGamePaused: false);

                    enemyManager.Draw(spriteBatch);
                    enemyManager.DrawUI(spriteBatch, defaultFont);

                    towerManager.Draw(spriteBatch, isGamePaused: false);

                    uiManager.Draw(spriteBatch);
                    base.Draw(gameTime);
                    break;

                case GameState.Paused:

                    spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    gridManager.Draw(spriteBatch, pixel, inputManager.HoveredCell, inputManager.MouseInsideGrid, isGamePaused: true);

                    enemyManager.Draw(spriteBatch);
                    enemyManager.DrawUI(spriteBatch, defaultFont);

                    towerManager.Draw(spriteBatch, isGamePaused: true);

                    base.Draw(gameTime);

                    pauseMenu.Draw(spriteBatch);
                    break;

                case GameState.GameOver:

                    gameOverScreen.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();
        }
    }
}
