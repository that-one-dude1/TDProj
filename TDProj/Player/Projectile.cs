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
    public class Projectile
    {
        private Vector2 position;
        private float speed;
        private float damage;
        private bool active = true;
        private Texture2D texture;
        private float lifeSpan;
        private List<Enemy> hitEnemies = new List<Enemy>();
        private Vector2 direction;

        public bool Active => active;

        public Projectile(Vector2 start, Vector2 targetPosition, float speed, float damage, Texture2D texture)
        {
            position = start;
            this.speed = speed;
            this.damage = damage;
            this.texture = texture;
            direction = targetPosition - position;
            direction.Normalize();
        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            lifeSpan += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (damage <= 0 || lifeSpan >= 2f)
            {
                active = false;
                return;
            }

            foreach (var enemy in enemies)
            {
                if (hitEnemies.Contains(enemy)) continue;

                Vector2 toEnemy = enemy.position - position;
                float distance = toEnemy.Length();

                if (distance < 40f)
                {
                    float damageDealt = Math.Min(damage, enemy.health);
                    damage -= damageDealt;
                    enemy.TakeDamage(damageDealt);
                    hitEnemies.Add(enemy);
                }
            }

            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!active) return;

            Rectangle rect = new Rectangle((int)position.X - 3, (int)position.Y - 3, 6, 6);
            spriteBatch.Draw(texture, rect, Color.Orange);
        }
    }
}
