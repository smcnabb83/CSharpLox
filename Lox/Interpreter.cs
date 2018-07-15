using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lox;
using Lox.BuiltInFunctions;
using tt = Lox.Token.TokenType;

namespace Lox
{
    public class Interpreter : GExpr.Visitor<Object>, GStmt.Visitor<Object>
    {
        public Environment globals = new Environment();
        private Environment environment;
        private Dictionary<GExpr.Expr, int> locals = new Dictionary<GExpr.Expr, int>();

        private Object evaluate(GExpr.Expr Expression)
        {
            return Expression.Accept(this);
        }

        private void execute(GStmt.Stmt st)
        {
            st.Accept(this);
        }

        public Interpreter()
        {
            environment = globals;

            globals.define("clock", new Function_Clock());
        }

        public void executeBlock(List<GStmt.Stmt> statements, Environment env)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = env;

                foreach(var statement in statements)
                {
                    execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
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
            return lookUpVariable(expr.name, expr);
        }

        private Object lookUpVariable(Token name, GExpr.Expr expression)
        {
            int distance = locals.ContainsKey(expression) ? locals[expression] : -1;
            if(distance >= 0)
            {
                return environment.getAt(distance, name.lexeme);
            }
            else
            {
                return globals.get(name);
            }
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

            int distance = locals.ContainsKey(expr) ? locals[expr] : -1;
            if(distance >= 0)
            {
                environment.assignAt(distance, expr.name, value);
            } else
            {
                globals.assign(expr.name, value);
            }

            return value;
        }

        public Object visit_Block_Stmt(GStmt.Block stmt)
        {
            executeBlock(stmt.statements, new Environment(environment));
            return null;
        }

        public Object visit_If_Stmt(GStmt.If stmt)
        {
            if (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.thenBranch);
            } 
            else if(stmt.elseBranch != null)
            {
                execute(stmt.elseBranch);
            }

            return null;
        }

        public Object visit_Logical_Expr(GExpr.Logical expr)
        {
            Object left = evaluate(expr.left);

            if(expr.Operator.type == tt.OR)
            {
                if (isTruthy(left))
                {
                    return left;
                }
            } else
            {
                if (!isTruthy(left))
                {
                    return left;
                }
            }

            return evaluate(expr.right);
        }

        public Object visit_While_Stmt(GStmt.While stmt)
        {
            while (isTruthy(evaluate(stmt.condition)))
            {
                try
                {
                    execute(stmt.body);
                }
                catch(BreakException bex)
                {
                    break;
                }
            }
            return null;
        }

        public object visit_Break_Stmt(GStmt.Break stmt)
        {
            throw new BreakException(stmt.breakToken, "Invalid break");
        }

        public object visit_Call_Expr(GExpr.Call expr)
        {
            Object callee = evaluate(expr.callee);

            List<object> arguments = new List<object>();

            foreach(GExpr.Expr argument in expr.Arguments)
            {
                arguments.Add(evaluate(argument));
            }

            if(!(callee is LoxCallable))
            {
                throw new RuntimeError(expr.paren, "Can only call functions and classes");
            }

            LoxCallable function = (LoxCallable)callee;

            if(arguments.Count != function.arity())
            {
                throw new RuntimeError(expr.paren, $"Expected {function.arity()} arguments but got {arguments.Count}.");
            }
            return function.call(this, arguments);
        }

        public object visit_Function_Stmt(GStmt.Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, environment, false);
            environment.define(stmt.name.lexeme, function);
            return null;
        }

        public object visit_Return_Stmt(GStmt.Return stmt)
        {
            Object value = null;
            if(stmt.value != null)
            {
                value = evaluate(stmt.value);
            }
            throw new Returner(value);
        }

        public void resolve(GExpr.Expr expr, int depth)
        {
            locals.Add(expr, depth);
        }

        public object visit_Class_Stmt(GStmt.Class stmt)
        {
            environment.define(stmt.name.lexeme, null);

            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
            foreach(GStmt.Function method in stmt.methods)
            {
                LoxFunction function = new LoxFunction(method, environment, method.name.lexeme == "init");
                methods.Add(method.name.lexeme, function);
            }

            LoxClass klass = new LoxClass(stmt.name.lexeme, methods);
            environment.assign(stmt.name, klass);
            return null;
        }

        public object visit_Get_Expr(GExpr.Get expr)
        {
            object obj = evaluate(expr.Object);
            if(obj is LoxInstance)
            {
                return ((LoxInstance)obj).get(expr.name);
            }

            throw new RuntimeError(expr.name, "Only instances have properties");
        }

        public object visit_Set_Expr(GExpr.Set expr)
        {
            object obj = evaluate(expr.Object);

            if(!(obj is LoxInstance))
            {
                throw new RuntimeError(expr.name, "Only instances have fields");
            }
            Object value = evaluate(expr.value);
            ((LoxInstance)obj).set(expr.name, value);

            return null;
        }

        public object visit_This_Expr(GExpr.This expr)
        {
            return lookUpVariable(expr.keyword, expr);
        }
    }
}
