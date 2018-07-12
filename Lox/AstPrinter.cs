using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    /*
    class AstPrinter : GExpr.Visitor<string>
    {

        public string print(GExpr.Expr expression)
        {
            return expression.Accept(this);
        }

        public string visit_Binary_Expr(GExpr.Binary expr)
        {
            return parenthesize(expr.Operator.lexeme, expr.left, expr.right);
        }

        public string visit_Grouping_Expr(GExpr.Grouping expr)
        {
            return parenthesize("group", expr.expression);
        }

        public string visit_Literal_Expr(GExpr.Literal expr)
        {
            if (expr.value == null)
            {
                return "nil";
            }
            return expr.value.ToString();
        }

        public string visit_Unary_Expr(GExpr.Unary expr)
        {
            return parenthesize(expr.Operator.lexeme, expr.right);
        }

        private string parenthesize(String name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(").Append(name);
            foreach(Expr expression in exprs)
            {
                builder.Append(" ");
                builder.Append(expression.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }*/
}
