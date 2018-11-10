using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeEscape
{
    public static class AppConfig
    {
        public static int WIDTH = 1280;
        public static int HEIGHT = 720;
        public static bool FULL_SCREEN = false;
        public static bool IS_MOUSE_VISIBLE = true;
        public static bool ALLOW_RESIZING = false;

        public static bool PLAY_SOUNDS = false;

        public static bool _DEBUG_SKIP_MAIN_MENU_ = true;//=false; // Pomijanie menu
        public static bool _DEBUG_AUTO_NO_CLIP_ = true;//=false; // PRzechodzenie przez ściany
        public static bool _DEBUG_SHOW_DIRECTION_TO_CENTER_ = false;
        public static bool _DEBUG_DISABLE_START_SPAWN_ = true; // POminięcie respawnu na początkowej drabince


        public static bool _DEBUG_DISABLE_COLLECTABLES_CHECK_ = false; // POminięcie respawnu na początkowej drabince

        public static int INITED_MESHES = 0;

        //public static ;
        //public static bool ;
        //public static int ;
        //public static TYPE NAME = VALUE;
    }
}
