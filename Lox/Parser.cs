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

        public Parser(List<Token> tokens)
        {
            this.Tokens = tokens;
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
            if (Match(tt.PRINT))
            {
                return printStatement();
            }
            if (Match(tt.LEFT_BRACE))
            {
                return new GStmt.Block(block());
            }

            return expressionStatement();
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
            return new GStmt.Expression(expr);
        }


        private GExpr.Expr expression()
        {
            return assignment();
        }

        private GExpr.Expr assignment()
        {
            GExpr.Expr expr = equality();

            if (Match(tt.EQUAL))
            {
                Token equals = previous();
                GExpr.Expr value = assignment();

                if(expr is GExpr.Variable)
                {
                    Token name = ((GExpr.Variable)expr).name;
                    return new GExpr.Assign(name, value);
                }

                error(equals, "Invalid assignment target.");
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

            return primary();
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
