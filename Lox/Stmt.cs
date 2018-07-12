using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lox.GExpr;

namespace Lox.GStmt
{
    public interface Visitor<T>
    {
        T visit_Expression_Stmt(Expression stmt);
        T visit_Print_Stmt(Print stmt);
        T visit_Var_Stmt(Var stmt);
        T visit_Block_Stmt(Block stmt);
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
    public class Var : Stmt
    {

        public Token name;
        public Expr initializer;

        public Var(Token name, Expr initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Var_Stmt(this);
        }
    }
    public class Block : Stmt
    {

        public List<Stmt> statements;

        public Block(List<Stmt> statements)
        {
            this.statements = statements;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Block_Stmt(this);
        }
    }
}