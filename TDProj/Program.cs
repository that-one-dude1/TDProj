using System;
using System.Windows.Forms; // needed for dialogs

namespace TDProj
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable Windows-style controls (for dialogs)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
}