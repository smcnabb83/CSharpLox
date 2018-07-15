using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lox;
using tt = Lox.Token.TokenType;

namespace Lox
{
    public class Parser
    {
        private List<Token> Tokens;
        private int current = 0;
        private bool isRepl = false;

        public Parser(List<Token> tokens, bool repl = false)
        {
            this.Tokens = tokens;
            this.isRepl = repl;
        }

        public List<GStmt.Stmt> parse()
        {

            List<GStmt.Stmt> statements = new List<GStmt.Stmt>();
            while (!isAtEnd())
            {
                statements.Add(declaration());
            }
            return statements;

        }

        private GStmt.Stmt declaration()
        {
            try
            {
                if (Match(tt.CLASS))
                {
                    return classDeclaration();
                }
                if (Match(tt.FUN))
                {
                    return function("function");
                }
                if (Match(tt.VAR))
                {
                    return varDeclaration();
                }
                return statement();
            }
            catch(ParseException error)
            {
                synchronize();
                return null;
            }
        }

        private GStmt.Stmt classDeclaration()
        {
            Token name = consume(tt.IDENTIFIER, "Expect class name");
            consume(tt.LEFT_BRACE, "Expect '{' before class body");

            List<GStmt.Function> methods = new List<GStmt.Function>();
            while(!check(tt.RIGHT_BRACE) && !isAtEnd())
            {
                methods.Add(function("method") as GStmt.Function);
            }

            consume(tt.RIGHT_BRACE, "Expect '}' after class body.");

            return new GStmt.Class(name, methods);
        }

        private GStmt.Stmt function(String kind)
        {
            Token name = consume(tt.IDENTIFIER, $"Expect {kind} name");
            consume(tt.LEFT_PAREN, $"Expect '(' after {kind} name");
            List<Token> parameters = new List<Token>();
            {
                if (!check(tt.RIGHT_PAREN))
                {
                    do
                    {
                        if (parameters.Count >= 8)
                        {
                            error(peek(), "Cannot have more than 8 parameters");
                        }

                        parameters.Add(consume(tt.IDENTIFIER, "Expect parameter name"));
                    } while (Match(tt.COMMA));

                }

                consume(tt.RIGHT_PAREN, "Expect ')' after parameters.");

                consume(tt.LEFT_BRACE, "Expect '{' before " + kind + " body.");
                List<GStmt.Stmt> body = block();
                return new GStmt.Function(name, parameters, body);
            }
        }

        private GStmt.Stmt varDeclaration()
        {
            Token name = consume(tt.IDENTIFIER, "Expect variable name");

            GExpr.Expr initializer = null;
            if (Match(tt.EQUAL))
            {
                initializer = expression();
            }

            consume(tt.SEMICOLON, "Expect ';' after variable declaration");
            return new GStmt.Var(name, initializer);
        }

        private GStmt.Stmt statement()
        {
            if (Match(tt.IF))
            {
                return ifStatement();
            }
            if (Match(tt.PRINT))
            {
                return printStatement();
            }
            if (Match(tt.RETURN))
            {
                return returnStatement();
            }
            if (Match(tt.FOR))
            {
                return forStatement();
            }
            if (Match(tt.WHILE))
            {
                return whileStatement();
            }
            if (Match(tt.LEFT_BRACE))
            {
                return new GStmt.Block(block());
            }
            if (Match(tt.BREAK))
            {
                return breakStatement();
            }

            return expressionStatement();
        }

        private GStmt.Stmt returnStatement()
        {
            Token keyword = previous();
            GExpr.Expr value = null;

            if (!check(tt.SEMICOLON))
            {
                value = expression();
            }

            consume(tt.SEMICOLON, "Expect ';' after return value");
            return new GStmt.Return(keyword, value);
        }

        private GStmt.Stmt breakStatement()
        {
            consume(tt.SEMICOLON, "Expect semicolon after break statement");
            return new GStmt.Break(previous());
        }

        private GStmt.Stmt forStatement()
        {
            consume(tt.LEFT_PAREN, "Expect '(' after 'for'.");

            GStmt.Stmt initializer;
            if (Match(tt.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(tt.VAR))
            {
                initializer = varDeclaration();
            }
            else
            {
                initializer = expressionStatement();
            }

            GExpr.Expr condition = null;
            if (!check(tt.SEMICOLON))
            {
                condition = expression();
            }
            consume(tt.SEMICOLON, "Expect ';' after loop condition.");

            GExpr.Expr increment = null;
            if (!check(tt.RIGHT_PAREN))
            {
                increment = expression();
            }
            consume(tt.RIGHT_PAREN, "Expect ')' after for clauses");
            GStmt.Stmt body = statement();
            if (increment != null)
            {
                body = new GStmt.Block(new List<GStmt.Stmt>() { body, new GStmt.Expression(increment) });
            }
            if(condition == null)
            {
                condition = new GExpr.Literal(true);
            }
            body = new GStmt.While(condition, body);

            if(initializer != null)
            {
                body = new GStmt.Block(new List<GStmt.Stmt>() { initializer, body });
            }
        

            return body;

        }

        private GStmt.Stmt whileStatement()
        {
            consume(tt.LEFT_PAREN, "Expect '(' after 'while'");
            GExpr.Expr condition = expression();
            consume(tt.RIGHT_PAREN, "Expect ')' after condition");
            GStmt.Stmt body = statement();

            return new GStmt.While(condition, body);
        }

        private List<GStmt.Stmt> block()
        {
            List<GStmt.Stmt> statements = new List<GStmt.Stmt>();
            while(!check(tt.RIGHT_BRACE) && !isAtEnd())
            {
                statements.Add(declaration());
            }
            consume(tt.RIGHT_BRACE, "Expect } after block.");
            return statements;
        }

        private GStmt.Stmt ifStatement()
        {
            consume(tt.LEFT_PAREN, "Expect ( after 'if'");
            GExpr.Expr condition = expression();
            consume(tt.RIGHT_PAREN, "Expect ) after if condition.");

            GStmt.Stmt thenBranch = statement();
            GStmt.Stmt elseBranch = null;
            if (Match(tt.ELSE))
            {
                elseBranch = statement();
            }

            return new GStmt.If(condition, thenBranch, elseBranch);
        }

        private GStmt.Stmt printStatement()
        {
            GExpr.Expr value = expression();
            consume(tt.SEMICOLON, "Expect ';' after value.");
            return new GStmt.Print(value);
        }
        
        private GStmt.Stmt expressionStatement()
        {
            GExpr.Expr expr = expression();
            consume(tt.SEMICOLON, "Expect ';' after value.");
            if (isRepl)
            {
                return new GStmt.Print(expr);
            }
            return new GStmt.Expression(expr);
        }


        private GExpr.Expr expression()
        {
            return assignment();
        }

        private GExpr.Expr assignment()
        {
            GExpr.Expr expr = or();

            if (Match(tt.EQUAL))
            {
                Token equals = previous();
                GExpr.Expr value = assignment();

                if(expr is GExpr.Variable)
                {
                    Token name = ((GExpr.Variable)expr).name;
                    return new GExpr.Assign(name, value);
                } else if (expr is GExpr.Get)
                {
                    GExpr.Get get = (GExpr.Get)expr;
                    return new GExpr.Set(get.Object, get.name, value);
                }

                error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private GExpr.Expr or()
        {
            GExpr.Expr expr = and();

            while (Match(tt.OR))
            {
                Token op = previous();
                GExpr.Expr right = and();
                expr = new GExpr.Logical(expr, op, right);
            }

            return expr;
        }

        private GExpr.Expr and()
        {
            GExpr.Expr expr = equality();

            while (Match(tt.AND))
            {
                Token op = previous();
                GExpr.Expr right = equality();
                expr = new GExpr.Logical(expr, op, right);
            }

            return expr;
        }

        private GExpr.Expr equality()
        {
            GExpr.Expr expr = comparison();
            while(Match(tt.BANG_EQUAL, tt.EQUAL_EQUAL))
            {
                Token Operator = previous();
                GExpr.Expr right = comparison();
                expr = new GExpr.Binary(expr, Operator, right);
            }

            return expr;
        }

        private GExpr.Expr comparison()
        {
            GExpr.Expr expr = addition();

            while (Match(tt.GREATER, tt.GREATER_EQUAL, tt.LESS, tt.LESS_EQUAL))
            {
                Token Operator = previous();
                GExpr.Expr right = addition();
                expr = new GExpr.Binary(expr, Operator, right);
            }

            return expr;
        }

        private GExpr.Expr addition()
        {
            GExpr.Expr expr = multiplication();

            while(Match(tt.MINUS, tt.PLUS))
            {
                Token Operator = previous();
                GExpr.Expr right = multiplication();
                expr = new GExpr.Binary(expr, Operator, right);
            }

            return expr;
        }

        private GExpr.Expr multiplication()
        {
            GExpr.Expr expr = unary();

            while(Match(tt.SLASH, tt.STAR))
            {
                Token Operator = previous();
                GExpr.Expr right = unary();
                expr = new GExpr.Binary(expr, Operator, right);
            }

            return expr;
        }

        private GExpr.Expr unary()
        {
            if(Match(tt.BANG, tt.MINUS))
            {
                Token Operator = previous();
                GExpr.Expr right = unary();
                return new GExpr.Unary(Operator, right);
            }

            return call();
        }

        private GExpr.Expr call()
        {
            GExpr.Expr expr = primary();

            while (true)
            {
                if (Match(tt.LEFT_PAREN))
                {
                    expr = finishCall(expr);
                }
                else if (Match(tt.DOT))
                {
                    Token name = consume(tt.IDENTIFIER, "Expect property name after '.'.");
                    expr = new GExpr.Get(expr, name);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private GExpr.Expr finishCall(GExpr.Expr callee)
        {
            List<GExpr.Expr> arguments = new List<GExpr.Expr>();
            if (!check(tt.RIGHT_PAREN))
            {
                do
                {
                    if(arguments.Count >= 8)
                    {
                        error(peek(), "Cannot have more than 8 arguments");
                    }
                    arguments.Add(expression());
                } while (Match(tt.COMMA));
            }

            Token paren = consume(tt.RIGHT_PAREN, "Expect ')' after arguments");
            return new GExpr.Call(callee, paren, arguments);
        }

        private GExpr.Expr primary()
        {
            if (Match(tt.FALSE))
            {
                return new GExpr.Literal(false);
            }

            if (Match(tt.TRUE))
            {
                return new GExpr.Literal(true);
            }

            if (Match(tt.NIL))
            {
                return new GExpr.Literal(null);
            }

            if (Match(tt.NUMBER, tt.STRING))
            {
                return new GExpr.Literal(previous().literal);
            }

            if (Match(tt.LEFT_PAREN))
            {
                GExpr.Expr expr = expression();
                consume(tt.RIGHT_PAREN, "Expect ')' after expression.");
                return new GExpr.Grouping(expr);
            }

            if (Match(tt.THIS))
            {
                return new GExpr.This(previous());
            }
            if (Match(tt.IDENTIFIER))
            {
                return new GExpr.Variable(previous());
            }

            throw error(peek(), "unexpected token");

        }

        private Token consume(tt type, string message)
        {
            if (check(type))
            {
                return advance();
            }

            throw error(peek(), message);
        }

        private ParseException error(Token token, string message)
        {
            Program.error(token, message);
            return new ParseException();
        }

        private void synchronize()
        {
            advance();

            while (!isAtEnd())
            {
                if(previous().type == tt.SEMICOLON)
                {
                    return;
                }

                switch (peek().type)
                {
                    case tt.CLASS:
                        return;
                    case tt.FUN:
                        return;
                    case tt.VAR:
                        return;
                    case tt.FOR:
                        return;
                    case tt.IF:
                        return;
                    case tt.WHILE:
                        return;
                    case tt.PRINT:
                        return;
                    case tt.RETURN:
                        return;
                }

                advance();
            }
        }

        private bool Match(params Token.TokenType[] types)
        {
            foreach(var type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }

            return false;
        }

        private bool check(Token.TokenType tokenType)
        {
            if (isAtEnd())
            {
                return false;
            }
            return peek().type == tokenType;
        }

        private Token advance()
        {
            if (!isAtEnd())
            {
                current++;
            }
            return previous();
        }

        private Token peek()
        {
            return Tokens[current];
        }
        
        private bool isAtEnd()
        {
            return peek().type == Token.TokenType.EOF;
        }

        private Token previous()
        {
            return Tokens[current - 1];
        }

    }
}
