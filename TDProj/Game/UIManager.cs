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
    public class UIManager
    {
        private readonly Rectangle uiRect;
        private readonly Texture2D texture;
        private readonly SpriteFont font;
        private readonly PlayerResources playerResources;
        private readonly TowerManager towerManager;

        private Dictionary<TowerType, Rectangle> towerButtons = new();
        private Rectangle upgradeButton;
        private Rectangle sellButton;
        public TowerType? selectedTowerType = null;

        public bool visible;
        public bool placeMode = false;
        public bool upgradeMode = false;
        public bool isVisible;
        public bool drawEndWaveText;
        public int endWaveMoney;

        public bool Visible => visible;

        public UIManager(Rectangle rect, Texture2D texture, SpriteFont font, PlayerResources resources, TowerManager towerManager)
        {
            uiRect = rect;
            this.texture = texture;
            this.font = font;
            playerResources = resources;
            this.towerManager = towerManager;

            InitializeTowerButtons();
        }

        private void InitializeTowerButtons()
        {
            int buttonSize = 100;
            int margin = 40;
            int y = uiRect.Y + 40;
            int x = uiRect.X + 60;

            towerButtons[TowerType.Basic] = new Rectangle(x, y, buttonSize, buttonSize);
            towerButtons[TowerType.Sniper] = new Rectangle(x + buttonSize + margin, y, buttonSize, buttonSize);
            towerButtons[TowerType.Rapid] = new Rectangle(x + 2 * (buttonSize + margin), y, buttonSize, buttonSize);

            upgradeButton = new Rectangle(x, y + 50, 250, 80);
            sellButton = new Rectangle(x, y + 175, 250, 80);
        }

        public void HandleInput(Point mousePoint, bool leftClicked, Point mouseCell)
        {
            if (!visible || !leftClicked)
                return;

            if (towerManager.SelectedTower != null)
            {
                //Tower mode
                if (upgradeButton.Contains(mousePoint))
                    towerManager.UpgradeSelectedTower();
                else if (sellButton.Contains(mousePoint))
                    towerManager.SellSelectedTower(mouseCell);
            }
            else
            {
                //Build mode
                foreach (var kvp in towerButtons)
                {
                    if (kvp.Value.Contains(mousePoint))
                    {
                        selectedTowerType = kvp.Key;
                        return;
                    }
                }
            }
        }

        public void Show() => visible = true;

        public void Hide() => visible = false;

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                isVisible = (upgradeMode != true || towerManager.SelectedTower != null);
                //Background bar
                if (isVisible)
                    spriteBatch.Draw(texture, uiRect, Color.Black * 0.6f);

                if (towerManager.SelectedTower != null)
                    DrawTowerUI(spriteBatch);
                else if (upgradeMode != true)
                    DrawBuildUI(spriteBatch);

                if (isVisible)
                {
                    spriteBatch.DrawString(font, $"Money: ${playerResources.Money}", new Vector2(uiRect.Right - 300, uiRect.Y + 60), Color.Gold);
                    spriteBatch.DrawString(font, $"Lives: {playerResources.Lives}", new Vector2(uiRect.Right - 300, uiRect.Y + 100), Color.Red);
                }
                if (drawEndWaveText && isVisible)
                    spriteBatch.DrawString(font, $"Earned ${endWaveMoney} for completing a wave!", new Vector2(1400, 1000), Color.Gold);

            }
        }

        private void DrawBuildUI(SpriteBatch spriteBatch)
        {
            foreach (var kvp in towerButtons)
            {
                TowerDefinition def = TowerDefinition.GetDefinition(kvp.Key);
                bool affordable = playerResources.Money >= def.Cost;

                Color buttonColor = affordable ? def.Color * 0.6f : Color.Gray * 0.6f;
                spriteBatch.Draw(texture, kvp.Value, buttonColor);

                spriteBatch.DrawString(font, kvp.Key.ToString(), new Vector2(kvp.Value.X + 5, kvp.Value.Y + 25), Color.White);
                spriteBatch.DrawString(font, $"${def.Cost}", new Vector2(kvp.Value.X + 15, kvp.Value.Y + 65), affordable ? Color.Yellow : Color.DarkGray);

                //Highlight if selected
                if (selectedTowerType == kvp.Key)
                {
                    Rectangle border = kvp.Value;
                    spriteBatch.Draw(texture, new Rectangle(border.X - 4, border.Y - 4, border.Width + 8, 4), Color.White);
                    spriteBatch.Draw(texture, new Rectangle(border.X - 4, border.Bottom, border.Width + 8, 4), Color.White);
                    spriteBatch.Draw(texture, new Rectangle(border.X - 4, border.Y - 4, 4, border.Height + 8), Color.White);
                    spriteBatch.Draw(texture, new Rectangle(border.Right, border.Y - 4, 4, border.Height + 8), Color.White);
                }
            }

            string label = selectedTowerType.HasValue ? $"Selected: {selectedTowerType}" : "Select a Tower";
            spriteBatch.DrawString(font, label, new Vector2(60, uiRect.Y + 170), Color.White);
        }

        private void DrawTowerUI(SpriteBatch spriteBatch)
        {
            var tower = towerManager.SelectedTower;
            bool affordable = playerResources.Money >= tower.UpgradeCost;
            spriteBatch.DrawString(font, $"Selected Tower: {tower.Type} (Lvl {((tower.Level < 5) ? tower.Level : "MAX")})",
                new Vector2(60, uiRect.Y + 20), Color.White);

            //Upgrade
            spriteBatch.Draw(texture, upgradeButton, (affordable && tower.Level < 5) ? Color.Green * 0.7f : Color.DarkGray * 0.7f);
            spriteBatch.DrawString(font, $"Upgrade ({((tower.Level < 5) ? "$" + tower.UpgradeCost : "MAX")})",
                new Vector2(upgradeButton.X + 10, upgradeButton.Y + 25), Color.White);

            //Sell
            spriteBatch.Draw(texture, sellButton, Color.Orange * 0.7f);
            spriteBatch.DrawString(font, $"Sell (${towerManager.totalCost})", new Vector2(sellButton.X + 10, sellButton.Y + 25), Color.White);
        }

        public TowerType? GetSelectedTowerType() => selectedTowerType;

        public bool IsPointInUI(Point p) => uiRect.Contains(p);
    }
}
