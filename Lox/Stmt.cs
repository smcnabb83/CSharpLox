using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.GStmt
{
    public interface Visitor<T>
    {
        T visit_Expression_Stmt(Expression stmt);
        T visit_Print_Stmt(Print stmt);
        T visit_Var_Stmt(Var stmt);
    }
    public abstract class Stmt
    {
        public abstract T Accept<T>(Visitor<T> visitor);
    }

    public class Expression : Stmt
    {

        public GExpr.Expr expression;

        public Expression(GExpr.Expr expression)
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

        public GExpr.Expr expression;

        public Print(GExpr.Expr expression)
        {
            this.expression = expression;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Print_Stmt(this);
        }
    }
    public class Var : Stmt
    {

        public Token name;
        public GExpr.Expr initializer;

        public Var(Token name, GExpr.Expr initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Var_Stmt(this);
        }
    }
}