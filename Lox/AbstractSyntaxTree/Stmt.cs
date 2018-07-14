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
        T visit_If_Stmt(If stmt);
        T visit_While_Stmt(While stmt);
        T visit_Break_Stmt(Break stmt);
        T visit_Function_Stmt(Function stmt);
        T visit_Return_Stmt(Return stmt);
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
    public class If : Stmt
    {

        public Expr condition;
        public Stmt thenBranch;
        public Stmt elseBranch;

        public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
        {
            this.condition = condition;
            this.thenBranch = thenBranch;
            this.elseBranch = elseBranch;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_If_Stmt(this);
        }
    }
    public class While : Stmt
    {

        public Expr condition;
        public Stmt body;

        public While(Expr condition, Stmt body)
        {
            this.condition = condition;
            this.body = body;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_While_Stmt(this);
        }
    }
    public class Break : Stmt
    {

        public Token breakToken;

        public Break(Token breakToken)
        {
            this.breakToken = breakToken;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Break_Stmt(this);
        }
    }
    public class Function : Stmt
    {

        public Token name;
        public List<Token> parameters;
        public List<Stmt> body;

        public Function(Token name, List<Token> parameters, List<Stmt> body)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Function_Stmt(this);
        }
    }
    public class Return : Stmt
    {

        public Token keyword;
        public Expr value;

        public Return(Token keyword, Expr value)
        {
            this.keyword = keyword;
            this.value = value;
        }

        override public T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visit_Return_Stmt(this);
        }
    }
}
