using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Expr parse()
        {
            try
            {
                return expression();
            }
            catch(ParseException ex)
            {
                return null;
            }
        }

        private Expr expression()
        {
            return equality();
        }

        private Expr equality()
        {
            Expr expr = comparison();
            while(Match(tt.BANG_EQUAL, tt.EQUAL_EQUAL))
            {
                Token Operator = previous();
                Expr right = comparison();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr comparison()
        {
            Expr expr = addition();

            while (Match(tt.GREATER, tt.GREATER_EQUAL, tt.LESS, tt.LESS_EQUAL))
            {
                Token Operator = previous();
                Expr right = addition();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr addition()
        {
            Expr expr = multiplication();

            while(Match(tt.MINUS, tt.PLUS))
            {
                Token Operator = previous();
                Expr right = multiplication();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr multiplication()
        {
            Expr expr = unary();

            while(Match(tt.SLASH, tt.STAR))
            {
                Token Operator = previous();
                Expr right = unary();
                expr = new Binary(expr, Operator, right);
            }

            return expr;
        }

        private Expr unary()
        {
            if(Match(tt.BANG, tt.MINUS))
            {
                Token Operator = previous();
                Expr right = unary();
                return new Unary(Operator, right);
            }

            return primary();
        }

        private Expr primary()
        {
            if (Match(tt.FALSE))
            {
                return new Literal(false);
            }

            if (Match(tt.TRUE))
            {
                return new Literal(true);
            }

            if (Match(tt.NIL))
            {
                return new Literal(null);
            }

            if (Match(tt.NUMBER, tt.STRING))
            {
                return new Literal(previous().literal);
            }

            if (Match(tt.LEFT_PAREN))
            {
                Expr expr = expression();
                consume(tt.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
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
