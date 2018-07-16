using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.BuiltInFunctions
{
    class Function_ClearScreen : LoxCallable
    {
        public int arity()
        {
            return 0;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            Console.Clear();
            return null;
        }
    }
}
