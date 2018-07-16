using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.BuiltInFunctions
{
    class Function_PrintColor : LoxCallable
    {

        private Dictionary<string, ConsoleColor> colors = new Dictionary<string, ConsoleColor>();

        public Function_PrintColor()
        {
            foreach(ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
            {
                colors.Add(color.ToString(), color);
            }
        }

        public int arity()
        {
            return 3;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {

            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;

            ConsoleColor foreground = ConsoleColor.White;
            ConsoleColor background = ConsoleColor.Black;

            if(arguments[1] != null)
            {
                if (colors.ContainsKey(arguments[1].ToString()))
                {
                    foreground = colors[arguments[1].ToString()];
                }
            }
            if(arguments[2] != null)
            {
                if (colors.ContainsKey(arguments[2].ToString()))
                {
                    background = colors[arguments[2].ToString()];
                }
            }

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(arguments[0].ToString());
            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
            return null;

        }
    }
}
