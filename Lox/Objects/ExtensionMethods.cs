using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public static class ExtensionMethods
    {
        public static bool isEmpty(this Stack<Dictionary<string, bool>> ts)
        {
            return ts.Count == 0;
        }
    }
}
