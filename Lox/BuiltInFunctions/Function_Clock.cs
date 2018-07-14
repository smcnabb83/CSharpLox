using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.BuiltInFunctions
{
    public class Function_Clock : LoxCallable
    {
        public int arity()
        {
            return 0;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            return (double)System.DateTime.Now.Millisecond / (double)1000;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}
