﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lox.GExpr;
using Lox.GStmt;

namespace Lox
{
    class Resolver : GExpr.Visitor<Object>, GStmt.Visitor<Object>
    {
        private Interpreter interpreter;
        private Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;
        private bool inLoop = false;

        private enum FunctionType
        {
            NONE,
            FUNCTION,
            METHOD,
            INITIALIZER
        }

        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }

        public Resolver(Interpreter intr)
        {
            interpreter = intr;
        }

        public object visit_Assign_Expr(Assign expr)
        {
            resolve(expr.value);
            resolveLocal(expr, expr.name);
            return null;
        }

        public object visit_Binary_Expr(Binary expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }

        public object visit_Block_Stmt(Block stmt)
        {
            beginScope();
            resolve(stmt.statements);
            endScope();
            return null;
        }

        public object visit_Break_Stmt(Break stmt)
        {
            if (!inLoop)
            {
                Program.error(stmt.breakToken, "Cannot use break outside of a loop");
            }
            return null;
        }

        public object visit_Call_Expr(Call expr)
        {
            resolve(expr.callee);

            foreach(var argument in expr.Arguments)
            {
                resolve(argument);
            }

            return null;
        }

        public object visit_Expression_Stmt(Expression stmt)
        {
            resolve(stmt.expression);
            return null;
        }

        public object visit_Function_Stmt(Function stmt)
        {
            declare(stmt.name);
            define(stmt.name);

            resolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        public object visit_Grouping_Expr(Grouping expr)
        {
            resolve(expr.expression);
            return null;
        }

        public object visit_If_Stmt(If stmt)
        {
            resolve(stmt.condition);
            resolve(stmt.thenBranch);
            if (stmt.elseBranch != null)
            {
                resolve(stmt.elseBranch);
            }
            return null;
        }

        public object visit_Literal_Expr(Literal expr)
        {
            return null;
        }

        public object visit_Logical_Expr(Logical expr)
        {
            resolve(expr.right);
            resolve(expr.left);
            return null;
        }

        public object visit_Print_Stmt(Print stmt)
        {
            resolve(stmt.expression);
            return null;
        }

        public object visit_Return_Stmt(Return stmt)
        {
            if(currentFunction == FunctionType.NONE)
            {
                Program.error(stmt.keyword, "Cannot return from top-level code");
            }

            if(stmt.value != null)
            {
                if(currentFunction == FunctionType.INITIALIZER)
                {
                    Program.error(stmt.keyword, "Cannot return a value from an initializer");
                }
                resolve(stmt.value);
            }
            return null;
        }

        public object visit_Unary_Expr(Unary expr)
        {
            resolve(expr.right);
            return null;
        }

        public object visit_Var_Stmt(Var stmt)
        {
            declare(stmt.name);
            if (stmt.initializer != null)
            {
                resolve(stmt.initializer);
            }
            define(stmt.name);
            return null;
        }

        public object visit_Variable_Expr(Variable expr)
        {
            if (!scopes.isEmpty() && scopes.Peek().ContainsKey(expr.name.lexeme) && scopes.Peek()[expr.name.lexeme] == false)
            {
                Program.error(expr.name, "Cannot read local variable in its own initializer.");
            }

            resolveLocal(expr, expr.name);
            return null;
        }

        public object visit_While_Stmt(While stmt)
        {
            bool currentLoop = inLoop;
            inLoop = true;
            resolve(stmt.condition);
            resolve(stmt.body);
            inLoop = currentLoop;
            return null;
        }

        private void beginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void declare(Token name)
        {
            if (scopes.isEmpty())
            {
                return;
            }            
            scopes.Peek().Add(name.lexeme, false);
        }

        private void define(Token name)
        {
            if (scopes.isEmpty())
            {
                return;
            }
            if (scopes.Peek().ContainsKey(name.lexeme))
            {
                scopes.Peek()[name.lexeme] = true;
            }
            else
            {
                scopes.Peek().Add(name.lexeme, true);
            }
        }

        private void endScope()
        {
            scopes.Pop();
        }

        public void resolve(List<GStmt.Stmt> statements)
        {
            foreach(var statement in statements)
            {
                resolve(statement);
            }
        }
        private void resolve(GStmt.Stmt stmt)
        {
            stmt.Accept(this);
        }
        private void resolve(GExpr.Expr expr)
        {
            expr.Accept(this);
        }

        private void resolveFunction(GStmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;
            beginScope();
            foreach(Token param in function.parameters)
            {
                declare(param);
                define(param);
            }
            resolve(function.body);
            endScope();
            currentFunction = enclosingFunction;
        }
        private void resolveLocal(GExpr.Expr expr, Token name)
        {
            for(int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme))
                {
                    interpreter.resolve(expr,i);
                    return;
                }
            }
        }

        public object visit_Class_Stmt(GStmt.Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;

            declare(stmt.name);

            if(stmt.superclass != null)
            {
                currentClass = ClassType.SUBCLASS;
                resolve(stmt.superclass);
            }
            define(stmt.name);

            if(stmt.superclass != null)
            {
                beginScope();
                scopes.Peek().Add("super", true);
            }
            beginScope();
            scopes.Peek().Add("this", true);

            foreach (GStmt.Function method in stmt.methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if(method.name.lexeme == "init")
                {
                    declaration = FunctionType.INITIALIZER;
                }
                resolveFunction(method, declaration);
            }

            endScope();

            if(stmt.superclass != null)
            {
                endScope();
            }

            currentClass = enclosingClass;

            return null;
        }

        public object visit_Get_Expr(GExpr.Get expr)
        {
            resolve(expr.Object);
            return null;
        }

        public object visit_Set_Expr(GExpr.Set expr)
        {
            resolve(expr.value);
            resolve(expr.Object);
            return null;
        }

        public object visit_This_Expr(GExpr.This expr)
        {
            if(currentClass == ClassType.NONE)
            {
                Program.error(expr.keyword, "Cannot use 'this' outside of a class.");
            }
            resolveLocal(expr, expr.keyword);
            return null;
        }

        public object visit_Super_Expr(GExpr.Super expr)
        {
            if(currentClass == ClassType.NONE)
            {
                Program.error(expr.keyword, "Cannot use 'super' outside of a class");
            } else if (currentClass != ClassType.SUBCLASS)
            {
                Program.error(expr.keyword, "Cannot use 'super' in a class with no superclass");
            }
            resolveLocal(expr, expr.keyword);
            return null;
        }

        public object visit_Try_Stmt(GStmt.Try stmt)
        {
            resolve(stmt.tryStmt);
            resolve(stmt.catchStmt);
            return null;
        }
    }
}
