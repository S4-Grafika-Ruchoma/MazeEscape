using System;
using MainMenu;

namespace MazeEscape
{
#if WINDOWS 
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var Menu = new MainMenu.MainMenu())
            {
                Menu.Run();
                if (Menu.runGame)
                {
                    using (var game = new Game1())
                    {
                        game.Run();
                    }
                } 

            }
        }
    }
#endif
}
