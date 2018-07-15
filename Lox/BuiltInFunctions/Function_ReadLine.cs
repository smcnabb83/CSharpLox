using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.BuiltInFunctions
{
    class Function_ReadLine : LoxCallable
    {
        public int arity()
        {
            return 1;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            Console.Write(arguments[0].ToString());
            string ret = Console.ReadLine();
            double retNumber;
            if(double.TryParse(ret, out retNumber))
            {
                return retNumber;
            }

            return ret;
        }
    }
}
