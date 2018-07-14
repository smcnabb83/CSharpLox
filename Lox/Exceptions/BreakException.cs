using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    class BreakException : RuntimeError
    {
        public BreakException(Token token, string message) : base(token, message)
        {
        }
    }
}
