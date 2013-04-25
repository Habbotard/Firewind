using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class Log
    {
        public static void Write(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[CONVERTER] ");
            Console.ResetColor();
            Console.Write(text + "\n");
        }
    }
}
