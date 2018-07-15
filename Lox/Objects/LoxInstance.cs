using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class LoxInstance
    {
        private LoxClass klass;
        private Dictionary<string, object> fields = new Dictionary<string, object>();

        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        public override string ToString()
        {
            return $"{klass.name} instance";
        }

        public object get(Token name)
        {
            if (fields.ContainsKey(name.lexeme))
            {
                return fields[name.lexeme];
            }

            LoxFunction method = klass.findMethod(this, name.lexeme);
            if(method != null)
            {
                return method;
            }

            throw new RuntimeError(name, $"Undefined properrty '{name.lexeme}'.");
        }

        public void set(Token name, object value)
        {
            fields.Add(name.lexeme, value);
        }
    }
}
