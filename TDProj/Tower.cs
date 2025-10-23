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
        private Vector2 position;
        private TowerDefinition def;
        private float fireCooldown = 0f;
        private Texture2D texture;
        private List<Projectile> projectiles = new();

        public Vector2 Position => position;
        public TowerType Type => def.Type;

        public Tower(Vector2 pos, TowerType type, Texture2D texture)
        {
            position = pos;
            def = TowerDefinition.GetDefinition(type);
            this.texture = texture;
        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            fireCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update existing projectiles
            foreach (var p in projectiles)
                p.Update(gameTime);
            projectiles.RemoveAll(p => !p.Active);

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

        private Enemy FindTarget(List<Enemy> enemies)
        {
            Enemy closest = null;
            float closestDist = def.Range;

            foreach (var e in enemies)
            {
                if (!e.IsAlive) continue;
                float dist = Vector2.Distance(e.position, position);
                if (dist <= def.Range && dist < closestDist)
                {
                    closest = e;
                    closestDist = dist;
                }
            }
            return closest;
        }

        private void FireAt(Enemy target)
        {
            projectiles.Add(new Projectile(position, target, speed: 400f, damage: def.Damage, texture));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw tower base
            float size = 40;
            Rectangle rect = new Rectangle((int)(position.X - size / 2), (int)(position.Y - size / 2), (int)size, (int)size);
            spriteBatch.Draw(texture, rect, def.Color);

            //Draw projectiles
            foreach (var p in projectiles)
                p.Draw(spriteBatch);
        }
    }
}
