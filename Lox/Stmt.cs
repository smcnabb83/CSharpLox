using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.Stmnt
{
    public interface Visitor<T>
    {
        T visit_Expression_Stmt(Expression stmt);
        T visit_Print_Stmt(Print stmt);
    }
    public abstract class Stmt
    {
        public abstract T Accept<T>(Visitor<T> visitor);
    }

    public class Expression : Stmt
    {

        public Expr expression;

        public Expression(Expr expression)
        {
            this.expression = expression;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Expression_Stmt(this);
        }
    }
    public class Print : Stmt
    {

        public Expr expression;

        public Print(Expr expression)
        {
            this.expression = expression;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Print_Stmt(this);
        }
    }
}