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
    public class Enemy
    {
        private readonly List<Point> path;
        private int currentTargetIndex = 0;
        public Vector2 position;
        private float speed;
        private float health;
        private float maxHealth;

        private readonly EnemyDefinition definition;
        private readonly int cellSize;
        private readonly Texture2D circleTexture;
        private readonly Texture2D triangleTexture;
        private readonly Texture2D pentagonTexture;
        public bool IsAlive => health > 0;
        public EnemyType Type => definition.Type;

        public Enemy(List<Point> path, Texture2D circleTexture, Texture2D triangleTexture, Texture2D pentagonTexture, int cellSize, EnemyDefinition def)
        {
            this.path = path;
            this.circleTexture = circleTexture;
            this.triangleTexture = triangleTexture;
            this.pentagonTexture = pentagonTexture;
            this.cellSize = cellSize;
            this.definition = def;


            this.speed = def.Speed;
            this.health = def.Health;
            this.maxHealth = def.Health;

            if (path.Count > 0)
                position = new Vector2(path[0].X * cellSize + cellSize / 2f, path[0].Y * cellSize + cellSize / 2f);
        }

        public void Update(GameTime gameTime)
        {
            if (!IsAlive || currentTargetIndex >= path.Count - 1)
                return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 targetPos = new Vector2(
                path[currentTargetIndex + 1].X * cellSize + cellSize / 2f,
                path[currentTargetIndex + 1].Y * cellSize + cellSize / 2f
            );

            Vector2 dir = targetPos - position;
            float dist = dir.Length();

            if (dist < 1f)
            {
                currentTargetIndex++;
                if (currentTargetIndex >= path.Count - 1)
                    return;
            }
            else
            {
                dir.Normalize();
                float moveDist = speed * delta;

                if (moveDist >= dist) //overshoot fix
                {
                    position = targetPos;
                    currentTargetIndex++;
                }
                else
                {
                    position += dir * moveDist;
                }
            }
        }

        public void TakeDamage(float dmg)
        {
            health -= dmg;
            if (health < 0) health = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsAlive) return;

            float size = cellSize * 0.6f;
            Rectangle rect = new Rectangle(
                (int)(position.X - size / 2f),
                (int)(position.Y - size / 2f),
                (int)size, (int)size
            );

            switch (definition.Type)
            {
                case EnemyType.Circle:
                    spriteBatch.Draw(circleTexture, rect, definition.Color);
                    break;
                case EnemyType.Triangle:
                    spriteBatch.Draw(triangleTexture, rect, definition.Color);
                    break;
                case EnemyType.Pentagon:
                    spriteBatch.Draw(pentagonTexture, rect, definition.Color);
                    break;
            }
        }
    }
}
