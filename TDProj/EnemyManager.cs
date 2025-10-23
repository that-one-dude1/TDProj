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
    public class EnemyManager
    {
        private readonly List<Enemy> enemies = new();
        private readonly List<Point> path;
        private readonly Texture2D circleTexture;
        private readonly Texture2D triangleTexture;
        private readonly Texture2D pentagonTexture;
        private readonly int cellSize;
        private readonly Dictionary<EnemyType, EnemyDefinition> definitions;

        private List<Wave> waves;
        private int currentWaveIndex = 0;
        private float spawnTimer = 0f;
        private int enemiesSpawned = 0;
        private bool waveActive = false;
        private float waveDelayTimer = 0f;
        private float waveDelay = 3f;
        private Random rng = new();

        public EnemyManager(List<Point> path, Texture2D circleTexture, Texture2D triangleTexture, Texture2D pentagonTexture, int cellSize)
        {
            this.path = path;
            this.circleTexture = circleTexture;
            this.triangleTexture = triangleTexture;
            this.pentagonTexture = pentagonTexture;
            this.cellSize = cellSize;
            this.definitions = EnemyDefinition.GetDefaultDefinitions();
            waves = Wave.CreateDefaultWaves();
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currentWaveIndex >= waves.Count)
                return;

            if (!waveActive)
            {
                waveDelayTimer += delta;
                if (waveDelayTimer >= waveDelay)
                    StartNextWave();
            }
            else
            {
                var wave = waves[currentWaveIndex];
                spawnTimer += delta;

                if (enemiesSpawned < wave.EnemyCount && spawnTimer >= wave.SpawnInterval)
                {
                    EnemyType type = GetEnemyTypeForWave(currentWaveIndex);
                    var def = definitions[type];
                    enemies.Add(new Enemy(path, circleTexture, triangleTexture, pentagonTexture, cellSize, def));
                    enemiesSpawned++;
                    spawnTimer = 0f;
                }

                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies[i].Update(gameTime);
                    if (!enemies[i].IsAlive)
                        enemies.RemoveAt(i);
                }

                if (enemiesSpawned >= wave.EnemyCount && enemies.Count == 0)
                {
                    waveActive = false;
                    currentWaveIndex++;
                    waveDelayTimer = 0f;
                }
            }
        }

        private void StartNextWave()
        {
            waveActive = true;
            enemiesSpawned = 0;
            spawnTimer = 0f;
        }

        private EnemyType GetEnemyTypeForWave(int waveIndex)
        {
            //Example rule: later waves get stronger variety
            if (waveIndex < 2) return EnemyType.Circle;
            if (waveIndex < 4) return rng.NextDouble() < 0.5 ? EnemyType.Circle : EnemyType.Triangle;
            return (EnemyType)rng.Next(0, 3); //random between Circle/Triangle/Pentagon
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
        }
    }
}
