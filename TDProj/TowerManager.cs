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
        private readonly List<Tower> towers = new();
        private readonly Texture2D texture;
        private readonly GridManager grid;
        private readonly HashSet<Point> occupiedCells = new();

        public TowerManager(GridManager grid, Texture2D texture)
        {
            this.grid = grid;
            this.texture = texture;
        }

        public void Update(GameTime gameTime, IReadOnlyList<Enemy> enemies)
        {
            foreach (var t in towers)
                t.Update(gameTime, new List<Enemy>(enemies));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var t in towers)
                t.Draw(spriteBatch);
        }

        public void TryPlaceTower(Point mouseCell, TowerType? selectedType)
        {
            if (!selectedType.HasValue) return;
            if (!grid.IsInsideGrid(mouseCell)) return;
            if (occupiedCells.Contains(mouseCell)) return;
            if (grid.GetCellType(mouseCell) != GridManager.CellType.Empty) return;

            Vector2 pos = new Vector2(mouseCell.X * grid.CellSize + grid.CellSize / 2f,
                                      mouseCell.Y * grid.CellSize + grid.CellSize / 2f);

            towers.Add(new Tower(pos, selectedType.Value, texture));
            occupiedCells.Add(mouseCell);
        }

        public List<Tower> GetTowers() => towers;
    }
}
