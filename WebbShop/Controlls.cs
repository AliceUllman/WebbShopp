using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public class Controls
    {
        static ConsoleKey key;
        public int Col { get; set; } = 0;
        public int Row { get; set; } = 0;
        public static bool Selecting { get; set; } = true;


        public void HandleSelectionInput(int maxRow, int maxCol)
        {
            key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.LeftArrow && Col > 0) Col--;
            if (key == ConsoleKey.RightArrow && Col < maxCol - 1) Col++;
            if (key == ConsoleKey.UpArrow && Row > 0) Row--;
            if (key == ConsoleKey.DownArrow && Row < maxRow - 1) Row++;
            if (key == ConsoleKey.Enter) { Selecting = false; }
        }
        public void Reset()
        {
            Col = 0; Row = 0;
            Selecting = true;
        }
    }
}
