using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class LoxClass : LoxCallable
    {
        public string name;
        private Dictionary<string, LoxFunction> methods;
        LoxClass superclass;

        public LoxClass(string n, LoxClass superclass, Dictionary<string, LoxFunction> methods)
        {
            this.name = n;
            this.methods = methods;
            this.superclass = superclass;
        }

        public int arity()
        {
            LoxFunction initializer = methods.ContainsKey("init") ? methods["init"] : null;
            if (initializer == null)
            {
                return 0;
            }
            return initializer.arity();
        }

        public LoxFunction findMethod(LoxInstance instance, string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name].bind(instance);
            }

            if(superclass != null)
            {
                return superclass.findMethod(instance, name);
            }

            return null;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction initializer = methods.ContainsKey("init") ? methods["init"] : null;
            if(initializer != null)
            {
                initializer.bind(instance).call(interpreter, arguments);
            }
            return instance;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
