using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    public static class EditSelect
    {
        static ConsoleKey key;
        public static int Col { get; set; } = 0;
        public static int Row { get; set; } = 0;
        public static bool Selecting { get; set; } = true;

        
        public static void HandleSelectionInput(int maxRow, int maxCol)
        {          
            key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.LeftArrow && Col > 0) Col--;
            if (key == ConsoleKey.RightArrow && Col < maxCol - 1) Col++;
            if (key == ConsoleKey.UpArrow && Row > 0) Row--;
            if (key == ConsoleKey.DownArrow && Row < maxRow - 1) Row++;
            if (key == ConsoleKey.Enter) { Selecting = false; }
            if (key == ConsoleKey.Escape) { return; }
        }
        public static void Reset() 
        { 
            Col = 0; Row = 0;
            Selecting = true;
        }
    }
}
