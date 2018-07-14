using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public interface LoxCallable
    {
        Object call(Interpreter interpreter, List<object> arguments);
        int arity();
    }
}
