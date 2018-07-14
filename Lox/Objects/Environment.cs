using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class Environment
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();
        Environment enclosing;

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public void define(string name, object value)
        {
            values.Add(name, value);
        }

        public object get(Token name)
        {
            if (values.ContainsKey(name.lexeme)){
                return values[name.lexeme];
            }

            if(enclosing != null)
            {
                return enclosing.get(name);
            }

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }

        public object getAt(int distance, string name)
        {
            return ancestor(distance).values[name];
        }

        public void assignAt(int distance, Token name, Object value)
        {
            ancestor(distance).values[name.lexeme] = value;
        }

        Environment ancestor(int distance)
        {
            Environment env = this;
            for(int i = 0; i < distance; i++)
            {
                env = env.enclosing;
            }
            return env;
        }

        public void assign(Token name, Object value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            if(enclosing != null)
            {
                enclosing.assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }

    }
}
