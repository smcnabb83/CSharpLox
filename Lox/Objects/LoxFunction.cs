using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class LoxFunction : LoxCallable
    {
        private GStmt.Function declaration;
        private Environment closure;
        public LoxFunction(GStmt.Function dec, Environment closure)
        {
            this.closure = closure;
            this.declaration = dec;
        }

        public object call(Interpreter interpreter, List<Object> arguments)
        {
            Environment env = new Environment(closure);
            for(int i = 0; i < declaration.parameters.Count; i++)
            {
                env.define(declaration.parameters[i].lexeme, arguments[i]);
            }

            try
            {
                interpreter.executeBlock(declaration.body, env);
            }
            catch (Returner retValue)
            {
                return retValue.value;
            }
            return null;
        }

        public int arity()
        {
            return declaration.parameters.Count;
        }
    }
}
