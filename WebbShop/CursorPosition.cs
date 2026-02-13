using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public static class CursorPosition
    {
        public static int MenuSpace { get; set; }
        public static int X { get; set; }
        public static int Y { get; set; } = 0;


        public static void NextRow(int Height) { X = MenuSpace; Y = Y + Height + 3; }
        public static void Reset() { X = MenuSpace; Y = 0; }
    }
}
