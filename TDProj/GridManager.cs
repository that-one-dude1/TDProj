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
    public class GridManager
    {
        public enum CellType { Empty, Path, Tower }

        private readonly int width;
        private readonly int height;
        private readonly int cellSize;
        private readonly CellType[,] grid;

        public int CellSize => cellSize;

        public GridManager(int width, int height, int cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            grid = new CellType[width, height];
        }

        public void SetPath(IEnumerable<Point> pathPoints)
        {
            foreach (var p in pathPoints)
                grid[p.X, p.Y] = CellType.Path;
        }

        public CellType GetCellType(Point mouseCell)
        {
            return grid[mouseCell.X, mouseCell.Y];
        }

        public bool IsInsideGrid(Point p) => p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height;

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, Point hoveredCell, bool mouseInsideGrid)
        {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Rectangle cellRect = new(x * cellSize, y * cellSize, cellSize, cellSize);
                    if (grid[x, y] == CellType.Path)
                        spriteBatch.Draw(pixel, cellRect, Color.Red * 0.5f);
                }

            if (mouseInsideGrid)
            {
                Rectangle highlight = new(hoveredCell.X * cellSize, hoveredCell.Y * cellSize, cellSize, cellSize);
                spriteBatch.Draw(pixel, highlight, Color.Cyan * 0.3f);
            }

            //Grid lines
            for (int x = 0; x <= width * cellSize; x += cellSize)
                spriteBatch.Draw(pixel, new Rectangle(x, 0, 1, height * cellSize), Color.LightGray);

            for (int y = 0; y <= height * cellSize; y += cellSize)
                spriteBatch.Draw(pixel, new Rectangle(0, y, width * cellSize, 1), Color.LightGray);

            spriteBatch.Draw(pixel, new Rectangle(width * cellSize - 1, 0, 1, height * cellSize), Color.LightGray);
            spriteBatch.Draw(pixel, new Rectangle(0, height * cellSize - 1, width * cellSize, 1), Color.LightGray);
        }
    }
}
