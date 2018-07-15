using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.BuiltInFunctions
{
    class Function_isNumeric : LoxCallable
    {
        public int arity()
        {
            return 1;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            if(arguments[0] is double)
            {
                return true;
            }
            return false;
        }
    }
}
