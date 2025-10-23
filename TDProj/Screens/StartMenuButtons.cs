using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDProj.Screens
{
    public enum MenuButtons
    {
        StartGame,
        LoadSave,
        Instructions,
        Exit
    }

    public class StartMenuButtons
    {
        public MenuButtons ButtonType { get; }
        public Vector2 Position { get; }
        public string Text { get; }
        public Rectangle Bounds { get; }

        public StartMenuButtons(Vector2 position, string text, Rectangle bounds)
        {
            Position = position;
            Text = text;
            Bounds = bounds;
        }

        public static Dictionary<MenuButtons, StartMenuButtons> GetMenuButtons()
        {
            return new Dictionary<MenuButtons, StartMenuButtons>
            {
                { MenuButtons.StartGame, new StartMenuButtons(new Vector2(335, 425), "Start Game", new Rectangle(335, 425, 195, 67)) },
                { MenuButtons.LoadSave, new StartMenuButtons(new Vector2(605, 490), "Load Save", new Rectangle(605, 490, 179, 67)) },
                { MenuButtons.Instructions, new StartMenuButtons(new Vector2(875, 565), "Instructions", new Rectangle(875, 565, 195, 67)) },
                { MenuButtons.Exit, new StartMenuButtons(new Vector2(1145, 640), "Save and Exit", new Rectangle(1145, 640, 210, 67)) }
            };
        }
    }
}
