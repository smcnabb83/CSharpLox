using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class Scanner
    {
        private string Source;
        private List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static readonly Dictionary<string, Token.TokenType> keywords = new Dictionary<string, Token.TokenType>()
        {
            {"and", Token.TokenType.AND },
            {"class", Token.TokenType.CLASS },
            {"else", Token.TokenType.ELSE },
            {"false", Token.TokenType.FALSE },
            {"for", Token.TokenType.FOR },
            {"fun", Token.TokenType.FUN },
            {"if", Token.TokenType.IF },
            {"nil", Token.TokenType.NIL },
            {"or", Token.TokenType.OR },
            {"print", Token.TokenType.PRINT },
            {"return", Token.TokenType.RETURN },
            {"super", Token.TokenType.SUPER },
            {"this", Token.TokenType.THIS },
            {"true", Token.TokenType.TRUE },
            {"var", Token.TokenType.VAR },
            {"while", Token.TokenType.WHILE },
            {"break", Token.TokenType.BREAK }
        };

        public Scanner(string source)
        {
            Source = source;
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(Token.TokenType.EOF, "", null, line));
            return tokens;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c)
            {
                case '(': addToken(Token.TokenType.LEFT_PAREN); break;
                case ')': addToken(Token.TokenType.RIGHT_PAREN); break;
                case '{': addToken(Token.TokenType.LEFT_BRACE); break;
                case '}': addToken(Token.TokenType.RIGHT_BRACE); break;
                case ',': addToken(Token.TokenType.COMMA); break;
                case '.': addToken(Token.TokenType.DOT); break;
                case '-': addToken(Token.TokenType.MINUS); break;
                case '+': addToken(Token.TokenType.PLUS); break;
                case ';': addToken(Token.TokenType.SEMICOLON); break;
                case '*': addToken(Token.TokenType.STAR); break;
                case '!': addToken(match('=') ? Token.TokenType.BANG_EQUAL : Token.TokenType.BANG); break;
                case '=': addToken(match('=') ? Token.TokenType.EQUAL_EQUAL : Token.TokenType.EQUAL); break;
                case '<': addToken(match('=') ? Token.TokenType.LESS_EQUAL : Token.TokenType.LESS); break;
                case '>': addToken(match('=') ? Token.TokenType.GREATER_EQUAL : Token.TokenType.GREATER); break;

                case '/':
                    if (match('/'))
                    {
                        while(peek() != '\n' && !isAtEnd())
                        {
                            advance();
                        }
                    }
                    //else if (match('*'))
                    //{
                    //    while (!isAtEnd())
                    //    {
                    //        if(match('*'))
                    //        {
                    //            if(match('/'))
                    //            {
                    //                advance();
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    else
                    {
                        addToken(Token.TokenType.SLASH);
                    }
                    break;
                case ' ': break;
                case '\r':  break;
                case '\t': break;
                case '\n': line++; break;
                case '"': processString(); break;


                default:
                    if (isDigit(c))
                    {
                        processNumber();
                    }
                    else if (isAlpha(c))
                    {
                        processIdentifier();
                    }
                    else
                    {
                        Lox.Program.error(line, "Unexpected Character");
                    }
                    break;

            }
        }

        private char advance()
        {
            current++;
            return Source[current-1];
        }

        private Boolean match(char expected)
        {
            if (isAtEnd())
            {
                return false;
            }

            if(Source[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        private void processString()
        {
            while(peek() != '"' && !isAtEnd())
            {
                if(peek() == '\n')
                {
                    line++;
                }
                advance();
            }

            if (isAtEnd())
            {
                Program.error(line, "Unterminated String");
                return;
            }

            //process closing "
            advance();
            string value = Source.Substring(start + 1 , current - start - 2);
            addToken(Token.TokenType.STRING, value);
        }

        private bool isDigit(char c)
        {
            return "0123456789".Contains(c);
        }

        private void processNumber()
        {
            while (isDigit(peek()))
            {
                advance();
            }

            if(peek() == '.' && isDigit(peekNext()))
            {
                advance();
                while (isDigit(peek()))
                {
                    advance();
                }
            }

            addToken(Token.TokenType.NUMBER, double.Parse(Source.Substring(start, current - start)));
        }

        private void processIdentifier()
        {
            while (isAlphaNumeric(peek()))
            {
                advance();
            }
            string text = Source.Substring(start, current - start);
            Token.TokenType type = keywords.ContainsKey(text) ? keywords[text] : Token.TokenType.IDENTIFIER;

            addToken(type);
        }

        private bool isAlpha(char c)
        {
            return "abcdefghijklmnopqrstuvwxyz_".Contains(c.ToString().ToLower());
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }

        private char peek()
        {
            if (isAtEnd())
            {
                return '\0';
            }
            return Source[current];
        }

        private char peekNext()
        {
            if (current + 1 >= Source.Length)
            {
                return '\0';
            }
            return Source[current + 1];
        }

        private char previous()
        {
            if(current > 0)
            {
                return Source[current - 1];
            }
            return Source[current];
        }

        private void addToken(Token.TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(Token.TokenType type, Object literal)
        {
            string text = Source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }

        private Boolean isAtEnd()
        {
            return current >= Source.Length;
        }
    }
}
