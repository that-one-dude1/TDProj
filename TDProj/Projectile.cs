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
        private Enemy target;
        private float speed;
        private float damage;
        private bool active = true;
        private Texture2D texture;

        public bool Active => active;

        public Projectile(Vector2 start, Enemy target, float speed, float damage, Texture2D texture)
        {
            position = start;
            this.target = target;
            this.speed = speed;
            this.damage = damage;
            this.texture = texture;
        }

        public void Update(GameTime gameTime)
        {
            if (target == null || !target.IsAlive)
            {
                active = false;
                return;
            }

            Vector2 toTarget = target.position - position;
            float distance = toTarget.Length();

            if (distance < 10f)
            {
                target.TakeDamage(damage);
                active = false;
                return;
            }

            toTarget.Normalize();
            position += toTarget * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!active) return;

            Rectangle rect = new Rectangle((int)position.X - 3, (int)position.Y - 3, 6, 6);
            spriteBatch.Draw(texture, rect, Color.Orange);
        }
    }
}
