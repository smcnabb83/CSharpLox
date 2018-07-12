﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lox;
using tt = Lox.Token.TokenType;

namespace Lox
{
    class Interpreter : GExpr.Visitor<Object>, GStmt.Visitor<Object>
    {
        private Environment environment = new Environment();

        private Object evaluate(GExpr.Expr Expression)
        {
            return Expression.Accept(this);
        }

        private void execute(GStmt.Stmt st)
        {
            st.Accept(this);
        }

        public void interpret(List<GStmt.Stmt> statements)
        {
            try
            {
                foreach(var statement in statements)
                {
                    execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Program.runtimeError(error);
            }
        }

        public object visit_Binary_Expr(GExpr.Binary expr)
        {
            Object left = evaluate(expr.left);
            Object right = evaluate(expr.right);

            switch (expr.Operator.type)
            {
                case tt.MINUS:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case tt.SLASH:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case tt.STAR:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case tt.PLUS:
                    if((left is double) && (right is double))
                    {
                        return (double)left + (double)right;
                    }
                    if((left is string) && (right is string))
                    {
                        return left.ToString() + right.ToString();
                    }
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings");
                case tt.GREATER:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case tt.GREATER_EQUAL:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case tt.LESS:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case tt.LESS_EQUAL:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                case tt.EQUAL_EQUAL:
                    return isEqual(left, right);
                case tt.BANG_EQUAL:
                    return !isEqual(left, right);
            }

            return null;
        }

        private void checkNumberOperands(Token Op, Object left, Object right)
        {
            if((left is double) && (right is double))
            {
                return;
            }
            throw new RuntimeError(Op, "Operands must be numbers");
        }

        private bool isEqual(Object a, Object b)
        {
            if(a == null && b == null)
            {
                return true;
            }
            if(a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public object visit_Grouping_Expr(GExpr.Grouping expr)
        {
            return evaluate(expr.expression);
        }

        public object visit_Literal_Expr(GExpr.Literal expr)
        {
            return expr.value;
        }

        public object visit_Unary_Expr(GExpr.Unary expr)
        {
            Object right = evaluate(expr.right);

            switch (expr.Operator.type)
            {
                case tt.MINUS:
                    checkNumberOperand(expr.Operator, right);
                    return -(double)right;
                case tt.BANG:
                    return !isTruthy(right);
            }

            return null;
        }

        private void checkNumberOperand(Token Operator, Object operand)
        {
            if(operand is double)
            {
                return;
            }
            throw new RuntimeError(Operator, "Operand must be a number");
        }

        private bool isTruthy(Object obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(obj is bool)
            {
                return (bool)obj;
            }
            return true;
        }

        public Object visit_Expression_Stmt(GStmt.Expression stmt)
        {
            evaluate(stmt.expression);
            return null;
        }

        public Object visit_Print_Stmt(GStmt.Print stmt)
        {
            Object value = evaluate(stmt.expression);
            Console.WriteLine(value.ToString());
            return null;
        }

        public Object visit_Variable_Expr(GExpr.Variable expr)
        {
            return environment.get(expr.name);
        }

        public Object visit_Var_Stmt(GStmt.Var stmt)
        {
            Object value = null;
            if(stmt.initializer != null)
            {
                value = evaluate(stmt.initializer);
            }

            environment.define(stmt.name.lexeme, value);
            return null;
        }

        public Object visit_Assign_Expr(GExpr.Assign expr)
        {
            Object value = evaluate(expr.value);
            environment.assign(expr.name, value);
            return value;
        }
    }
}