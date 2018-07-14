using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    class Returner : Exception 
    {
        public Object value;

        public Returner(Object value)
        {
            this.value = value;
        }
    }
}
