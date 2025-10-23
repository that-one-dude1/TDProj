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
    public class TowerManager
    {
        public List<Tower> towers = new();
        private readonly Texture2D texture;
        private Texture2D rangeTexture;
        private readonly GridManager grid;
        public HashSet<Point> occupiedCells = new();
        private PlayerResources playerResources;
        private int upgradeTotal = 0;
        public int totalCost = 0;

        public Tower SelectedTower { get; private set; }

        public UIManager uiManager;

        public TowerManager(GridManager grid, Texture2D texture, PlayerResources resources, Texture2D rangeTexture)
        {
            this.grid = grid;
            this.texture = texture;
            playerResources = resources;
            this.rangeTexture = rangeTexture;
        }

        public void Update(GameTime gameTime, IReadOnlyList<Enemy> enemies)
        {
            foreach (var t in towers)
            {
                t.Update(gameTime, new List<Enemy>(enemies));
                foreach (var p in t.projectiles)
                    p.Update(gameTime);
                t.projectiles.RemoveAll(p => !p.Active);
            }

            if (SelectedTower != null)
                totalCost = (int)(TowerDefinition.GetDefinition(SelectedTower.Type).Cost * 0.6f) + (int)(SelectedTower.upgradeTotal * 0.6f);
        }

        public static Tower FromData(TowerData data, Texture2D pixelTexture)
        {
            return new Tower
            (
                new Vector2(data.PosX, data.PosY),
                data.Type,
                pixelTexture,
                new Point(data.CellX, data.CellY),
                data.Level,
                data.UpgradeTotal
            );
        }

        public void Reset()
        {
            towers.Clear();
            occupiedCells.Clear();
            SelectedTower = null;
            upgradeTotal = 0;
            totalCost = 0;
        }

        private float SelectedTowerRange()
        {
            if (SelectedTower == null) return 0;
            var def = TowerDefinition.GetDefinition(SelectedTower.Type);
            return def.Range * (1f + (SelectedTower.Level - 1) * 0.15f);
        }

        public void Draw(SpriteBatch spriteBatch, bool isGamePaused)
        {
            if (SelectedTower != null && rangeTexture != null && uiManager.Visible)
            {
                float range = SelectedTowerRange();
                Vector2 pos = SelectedTower.Position;
                float diameter = range * 2;

                Rectangle rect = new Rectangle(
                    (int)(pos.X - range),
                    (int)(pos.Y - range),
                    (int)diameter,
                    (int)diameter
                );

                //Draw semi-transparent range indicator
                if (!isGamePaused)
                    spriteBatch.Draw(rangeTexture, rect, Color.Cyan * 0.2f);
            }

            foreach (var t in towers)
            {
                t.Draw(spriteBatch);
                foreach (var p in t.projectiles)
                    p.Draw(spriteBatch);
            }
        }

        public void TryPlaceTower(Point mouseCell, TowerType? selectedType)
        {
            if (!selectedType.HasValue) return;
            if (!grid.IsInsideGrid(mouseCell)) return;
            if (occupiedCells.Contains(mouseCell)) return;
            if (grid.GetCellType(mouseCell) != GridManager.CellType.Empty) return;

            TowerDefinition def = TowerDefinition.GetDefinition(selectedType.Value);

            if (!playerResources.Spend(def.Cost))
                return; //Not enough money

            Vector2 pos = new Vector2(mouseCell.X * grid.CellSize + grid.CellSize / 2f + 1,
                                      mouseCell.Y * grid.CellSize + grid.CellSize / 2f + 1); //+1 for correction onto grid

            towers.Add(new Tower(pos, selectedType.Value, texture, mouseCell));
            occupiedCells.Add(mouseCell);
        }

        public void HandleTowerClick(Point mousePoint)
        {
            foreach (var tower in towers)
            {
                if (tower.ContainsPoint(mousePoint))
                {
                    SelectedTower = tower;
                    uiManager.selectedTowerType = null;
                    uiManager.placeMode = false;
                    uiManager.Show();
                    return;
                }
            }
            SelectedTower = null;
        }

        public void UpgradeSelectedTower()
        {
            if (SelectedTower == null) return;
            if (SelectedTower.Level >= 5) return;
            int cost = SelectedTower.UpgradeCost;
            if (playerResources.Spend(cost))
            {
                SelectedTower.Upgrade();
                SelectedTower.upgradeTotal += cost;
            }
        }


        public void SellSelectedTower(Point mouseCell)
        {
            if (SelectedTower == null) return;
            int refund = (int)(TowerDefinition.GetDefinition(SelectedTower.Type).Cost * 0.6f) + (int)(SelectedTower.upgradeTotal * 0.6f);
            playerResources.Earn(refund);
            towers.Remove(SelectedTower);
            occupiedCells.Remove(SelectedTower.cell);
            SelectedTower = null;
        }


        public List<Tower> GetTowers() => towers;
    }
}
