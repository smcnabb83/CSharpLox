using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.GExpr
{
    public interface Visitor<T>
    {
        T visit_Binary_Expr(Binary expr);
        T visit_Grouping_Expr(Grouping expr);
        T visit_Literal_Expr(Literal expr);
        T visit_Unary_Expr(Unary expr);
        T visit_Variable_Expr(Variable expr);
        T visit_Assign_Expr(Assign expr);
        T visit_Logical_Expr(Logical expr);
        T visit_Call_Expr(Call expr);
        T visit_Get_Expr(Get expr);
        T visit_Set_Expr(Set expr);
        T visit_This_Expr(This expr);
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
    public class Variable : Expr
    {

        public Token name;

        public Variable(Token name)
        {
            this.name = name;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Variable_Expr(this);
        }
    }
    public class Assign : Expr
    {

        public Token name;
        public Expr value;

        public Assign(Token name, Expr value)
        {
            this.name = name;
            this.value = value;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Assign_Expr(this);
        }
    }
    public class Logical : Expr
    {

        public Expr left;
        public Token Operator;
        public Expr right;

        public Logical(Expr left, Token Operator, Expr right)
        {
            this.left = left;
            this.Operator = Operator;
            this.right = right;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Logical_Expr(this);
        }
    }
    public class Call : Expr
    {

        public Expr callee;
        public Token paren;
        public List<Expr> Arguments;

        public Call(Expr callee, Token paren, List<Expr> Arguments)
        {
            this.callee = callee;
            this.paren = paren;
            this.Arguments = Arguments;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Call_Expr(this);
        }
    }
    public class Get : Expr
    {

        public Expr Object;
        public Token name;

        public Get(Expr Object, Token name)
        {
            this.Object = Object;
            this.name = name;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Get_Expr(this);
        }
    }
    public class Set : Expr
    {

        public Expr Object;
        public Token name;
        public Expr value;

        public Set(Expr Object, Token name, Expr value)
        {
            this.Object = Object;
            this.name = name;
            this.value = value;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Set_Expr(this);
        }
    }
    public class This : Expr
    {

        public Token keyword;

        public This(Token keyword)
        {
            this.keyword = keyword;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_This_Expr(this);
        }
    }
}