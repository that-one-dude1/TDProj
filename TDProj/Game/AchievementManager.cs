using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDProj
{
    public class AchievementManager
    {
        private Texture2D pixel;
        private SpriteFont defaultFont;
        private SpriteFont smallFont;
        private PlayerResources playerResources;
        private EnemyManager enemyManager;
        private TowerManager towerManager;

        public AchievementManager(Texture2D pixel, SpriteFont defaultFont, SpriteFont smallFont, PlayerResources playerResources, EnemyManager enemyManager, TowerManager towerManager)
        {
            this.pixel = pixel;
            this.defaultFont = defaultFont;
            this.smallFont = smallFont;
            this.playerResources = playerResources;
            this.enemyManager = enemyManager;
            this.towerManager = towerManager;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw (SpriteBatch spriteBatch)
        {

        }
    }
}
