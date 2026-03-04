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
    public class EnemySpawner
    {
        private Wave currentWave;
        private EnemyManager enemyManager;
        private int groupIndex;
        private int spawnedInGroup;
        private float groupTimer;
        private float spawnTimer;
        public int totalSpawnNum;

        public EnemySpawner(EnemyManager enemyManager)
        {
            this.enemyManager = enemyManager;
        }

        public void BeginWave(Wave wave)
        {
            currentWave = wave;
            groupIndex = 0;
            spawnedInGroup = 0;
            groupTimer = 0f;
            spawnTimer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currentWave == null || groupIndex >= currentWave.SpawnGroups.Count)
                return;

            var group = currentWave.SpawnGroups[groupIndex];
            groupTimer += delta;
            if (groupTimer < group.StartDelay)
                return;

            spawnTimer += delta;

            if (spawnTimer >= group.Interval)
            {
                enemyManager.enemies.Add(new Enemy(
                    enemyManager.path,
                    enemyManager.circleTexture,
                    enemyManager.triangleTexture,
                    enemyManager.pentagonTexture,
                    enemyManager.hexagonTexture,
                    enemyManager.cellSize,
                    enemyManager.playerResources,
                    enemyManager.definitions[group.EnemyType]
                ));
                spawnTimer = 0f;
                spawnedInGroup++;
                totalSpawnNum++;

                if (spawnedInGroup >= group.Count)
                {
                    groupIndex++;
                    spawnedInGroup = 0;
                    spawnTimer = 0f;
                    groupTimer = 0f;
                }
            }
        }
    }

    public class EnemyManager
    {
        public List<Enemy> enemies = new();
        public readonly List<Point> path;
        public readonly Texture2D circleTexture;
        public readonly Texture2D triangleTexture;
        public readonly Texture2D pentagonTexture;
        public readonly Texture2D hexagonTexture;
        public readonly UIManager uiManager;
        public readonly int cellSize;
        public readonly Dictionary<EnemyType, EnemyDefinition> definitions;
        public EnemySpawner spawner;

        public PlayerResources playerResources;
        //private List<Wave> waves;
        private Wave wave = new();
        public int currentWaveIndex = 0;
        private float spawnTimer = 0f;
        private int enemiesSpawned = 0;
        private bool waveActive = false;
        private bool waveMoneyEarned = false;
        private float waveDelayTimer = 0f;
        private float waveDelay = 3f;
        private Random rng = new();
        public bool saveLoaded;

        public List<string> waveScript;

        public EnemyManager(List<Point> path, Texture2D circleTexture, Texture2D triangleTexture, Texture2D pentagonTexture, Texture2D hexagonTexture, UIManager uiManager, int cellSize, PlayerResources resources)
        {
            this.path = path;
            this.circleTexture = circleTexture;
            this.triangleTexture = triangleTexture;
            this.pentagonTexture = pentagonTexture;
            this.hexagonTexture = hexagonTexture;
            this.uiManager = uiManager;
            this.cellSize = cellSize;
            this.playerResources = resources;
            this.definitions = EnemyDefinition.GetDefaultDefinitions();
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //if (currentWaveIndex >= waves.Count)
            //    return;

            enemiesSpawned = spawner.totalEnemiesSpawned;

            if (!waveActive)
            {
                if (!waveMoneyEarned)
                {
                    playerResources.Earn((currentWaveIndex == 0) ? 0 : ((currentWaveIndex + 1) * 50));
                    waveMoneyEarned = true;
                }

                waveDelayTimer += delta;
                if (waveDelayTimer >= waveDelay)
                    StartNextWave(currentWaveIndex);
            }
            else
            {
                spawnTimer += delta;

                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies[i].Update(gameTime);
                    if (!enemies[i].IsAlive)
                    {
                        if (!enemies[i].ReachedEnd)
                            playerResources.Earn(definitions[enemies[i].Type].Reward); //reward per kill unless reached end

                        enemies.RemoveAt(i);
                    }
                }

                if (spawner.totalSpawnNum >= wave.TotalEnemyCount() && enemies.Count == 0)
                {
                    waveActive = false;
                    currentWaveIndex++;
                    waveDelayTimer = 0f;
                }
            }
        }

        public void Reset()
        {
            enemies.Clear();
            currentWaveIndex = 0;
            waveActive = false;
            waveMoneyEarned = false;
            enemiesSpawned = 0;
            spawner.totalEnemiesSpawned = 0;
            spawnTimer = 0f;
            waveDelayTimer = 0f;
            spawner.totalSpawnNum = 0;

            wave.SpawnGroups = new();
        }

        public void Reset(bool isSaveLoaded) //overload for in case a save is loaded
        {
            enemies.Clear();
            currentWaveIndex = 0;
            waveActive = false;
            waveMoneyEarned = true;
            enemiesSpawned = 0;
            spawner.totalEnemiesSpawned = 0;
            spawnTimer = 0f;
            waveDelayTimer = 0f;
            spawner.totalSpawnNum = 0;

            wave.SpawnGroups = new();

            saveLoaded = isSaveLoaded;
        }

        private void StartNextWave(int waveIndex)
        {
            waveActive = true;
            waveMoneyEarned = false;
            uiManager.drawEndWaveText = false;
            saveLoaded = false;
            enemiesSpawned = 0;
            spawner.totalEnemiesSpawned = 0;
            spawnTimer = 0f;
            spawner.totalSpawnNum = 0;

            wave.SpawnGroups = new();

            wave.ParseWave(waveScript[waveIndex]);
            spawner.BeginWave(wave);
        }

        public List<Enemy> GetEnemies()
        {
            return enemies;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var e in enemies)
                e.Draw(spriteBatch);
        }

        public void DrawUI(SpriteBatch spriteBatch, SpriteFont font)
        {
            string text = waveActive
                ? $"Wave {currentWaveIndex + 1} active: {enemies.Count} enemies"
                : $"Next wave in {Math.Max(0, Math.Ceiling(waveDelay - waveDelayTimer))}s";
            spriteBatch.DrawString(font, text, new Vector2(40, 40), Color.White);
            if (!waveActive && currentWaveIndex != 0 && uiManager.isVisible && !saveLoaded)
            {
                uiManager.endWaveMoney = (currentWaveIndex + 1) * 50;
                uiManager.drawEndWaveText = true;            
            }
        }
    }
}
