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

        private Dictionary<TowerType, Rectangle> towerButtons = new();
        public TowerType? selectedTowerType = null;

        bool visible;

        public bool Visible => visible;
        public TowerType? SelectedTowerType => selectedTowerType;

        public UIManager(Rectangle rect, Texture2D texture, SpriteFont font)
        {
            uiRect = rect;
            this.texture = texture;
            this.font = font;

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
        }

        public void HandleInput(Point mousePoint, bool leftClicked)
        {
            if (!visible || !leftClicked)
                return;

            foreach (var kvp in towerButtons)
            {
                if (kvp.Value.Contains(mousePoint) && selectedTowerType == null)
                {
                    selectedTowerType = kvp.Key;
                    Console.WriteLine($"Selected Tower: {selectedTowerType}");
                    return;
                }
            }
        }

        public void Show(Point cell)
        {
            Point selectedCell = cell;
            visible = true;
        }

        public void Hide() => visible = false;

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                //Background bar
                spriteBatch.Draw(texture, uiRect, Color.Black * 0.6f);

                foreach (var kvp in towerButtons)
                {
                    TowerDefinition def = TowerDefinition.GetDefinition(kvp.Key);
                    spriteBatch.Draw(texture, kvp.Value, def.Color * 0.6f);
                    spriteBatch.DrawString(font, kvp.Key.ToString(), new Vector2(kvp.Value.X + 10, kvp.Value.Y + 35), Color.White);

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
        }

        public bool IsPointInUI(Point p) => uiRect.Contains(p);
    }
}
