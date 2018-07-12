using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    class Environment
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public void define(string name, object value)
        {
            values.Add(name, value);
        }

        public object get(Token name)
        {
            if (values.ContainsKey(name.lexeme)){
                return values[name.lexeme];
            }

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }

        public void assign(Token name, Object value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }
    }
}
