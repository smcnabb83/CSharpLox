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
        private bool isInitializer;
        public LoxFunction(GStmt.Function dec, Environment closure, bool isInitializer)
        {
            this.closure = closure;
            this.declaration = dec;
            this.isInitializer = isInitializer;
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
                if (isInitializer)
                {
                    return closure.getAt(0, "this");
                }
                return retValue.value;
            }

            if (isInitializer)
            {
                return closure.getAt(0, "this");
            }
            return null;
        }

        public LoxFunction bind(LoxInstance instance)
        {
            Environment env = new Environment(closure);
            env.define("this", instance);
            return new LoxFunction(declaration, env, isInitializer);
        }

        public int arity()
        {
            return declaration.parameters.Count;
        }
    }
}
