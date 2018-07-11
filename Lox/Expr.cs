using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public interface Visitor<T>
    {
        T visit_Binary_Expr(Binary expr);
        T visit_Grouping_Expr(Grouping expr);
        T visit_Literal_Expr(Literal expr);
        T visit_Unary_Expr(Unary expr);
    }
    public abstract class Expr
    {
        public abstract T Accept<T>(Visitor<T> visitor);
    }

    public class Binary : Expr
    {

        public Expr left;
        public Token Operator;
        public Expr right;

        public Binary(Expr left, Token Operator, Expr right)
        {
            this.left = left;
            this.Operator = Operator;
            this.right = right;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Binary_Expr(this);
        }
    }
    public class Grouping : Expr
    {

        public Expr expression;

        public Grouping(Expr expression)
        {
            this.expression = expression;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Grouping_Expr(this);
        }
    }
    public class Literal : Expr
    {

        public Object value;

        public Literal(Object value)
        {
            this.value = value;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Literal_Expr(this);
        }
    }
    public class Unary : Expr
    {

        public Token Operator;
        public Expr right;

        public Unary(Token Operator, Expr right)
        {
            this.Operator = Operator;
            this.right = right;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Unary_Expr(this);
        }
    }
}
