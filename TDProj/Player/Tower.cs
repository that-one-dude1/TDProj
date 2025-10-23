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
    public class Tower
    {
        public Vector2 position { get; set; }
        public TowerDefinition def;
        private float fireCooldown = 0f;
        public Texture2D texture;
        public List<Projectile> projectiles = new();
        public Point cell;
        private int level = 1;
        public int upgradeTotal = 0;

        public Vector2 Position => position;
        public TowerType Type => def.Type;
        public int Level => level;
        public int UpgradeCost => (int)(TowerDefinition.GetDefinition(Type).Cost * (0.75f * level));

        public Tower(Vector2 pos, TowerType type, Texture2D texture, Point cell)
        {
            position = pos;
            def = TowerDefinition.GetDefinition(type);
            this.texture = texture;
            this.cell = cell;
        }

        public Tower(Vector2 pos, TowerType type, Texture2D texture, Point cell, int level, int upgradeTotal)
        {
            position = pos;
            def = TowerDefinition.GetDefinition(type);
            this.texture = texture;
            this.cell = cell;
            this.level = level;
            this.upgradeTotal = upgradeTotal;
            def = new TowerDefinition(
                def.Type,
                range: def.Range * (float)Math.Pow(1.15f, level),
                fireRate: def.FireRate * (float)Math.Pow(1.1f, level),
                damage: def.Damage * (float)Math.Pow(1.3f, level),
                cost: def.Cost,
                color: def.Color
            );
        }

        public TowerData ToData()
        {
            return new TowerData
            {
                PosX = this.position.X,
                PosY = this.position.Y,
                Type = this.def.Type,
                CellX = this.cell.X,
                CellY = this.cell.Y,
                Level = this.level,
                UpgradeTotal = this.upgradeTotal
            };
        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            fireCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fireCooldown <= 0)
            {
                var target = FindTarget(enemies);
                if (target != null)
                {
                    FireAt(target);
                    fireCooldown = 1f / def.FireRate;
                }
            }
        }

        public Enemy FindTarget(List<Enemy> enemies)
        {
            Enemy furthest = null;
            float maxProgress = -1f;

            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;

                //Only consider enemies within range
                float dist = Vector2.Distance(enemy.position, position);
                if (dist > def.Range) continue;

                //Pick the enemy that is furthest along the path
                if (enemy.ProgressAlongPath > maxProgress)
                {
                    maxProgress = enemy.ProgressAlongPath;
                    furthest = enemy;
                }
            }

            return furthest;
        }

        private void FireAt(Enemy target)
        {
            projectiles.Add(new Projectile(position, target, speed: 400f, damage: def.Damage, texture));
        }

        public void Upgrade()
        {
            level++;
            def = new TowerDefinition(
                def.Type,
                range: def.Range * 1.15f,
                fireRate: def.FireRate * 1.1f,
                damage: def.Damage * 1.3f,
                cost: def.Cost,
                color: def.Color
            );
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw tower base
            float size;
            if (level < 5)
            {
                size = 40 + (level - 1) * 4;
            }
            else
            {
                size = 59; //slightly smaller than grid size
            }
            Rectangle rect = new Rectangle((int)(position.X - size / 2), (int)(position.Y - size / 2), (int)size, (int)size);
            spriteBatch.Draw(texture, rect, def.Color);
        }

        public bool ContainsPoint(Point p)
        {
            Rectangle rect = new((int)(position.X - 30), (int)(position.Y - 30), 60, 60);
            return rect.Contains(p);
        }
    }
}
