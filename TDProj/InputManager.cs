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
    public class InputManager
    {
        public MouseState previousMouse;
        public MouseState CurrentMouse { get; private set; }

        public Point HoveredCell { get; private set; }
        public bool MouseInsideGrid { get; private set; }
        public bool LeftClicked { get; private set; }
        public Point MousePosition => new(CurrentMouse.X, CurrentMouse.Y);

        public Point cell;

        private readonly GridManager grid;

        public InputManager(GridManager grid)
        {
            this.grid = grid;
        }

        public void Update(MouseState mouse)
        {
            CurrentMouse = mouse;

            int cellX = mouse.X / grid.CellSize;
            int cellY = mouse.Y / grid.CellSize;
            Point cell = new(cellX, cellY);

            MouseInsideGrid = grid.IsInsideGrid(cell);
            if (MouseInsideGrid)
                HoveredCell = cell;

            LeftClicked = mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;
            previousMouse = mouse;
        }
    }
}
