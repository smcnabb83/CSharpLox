using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    public class Token
    {
        public enum TokenType
        {
            //Single-character tokens
            LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

            //One or two character tokens.
            BANG, BANG_EQUAL, EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

            //Literals
            IDENTIFIER, STRING, NUMBER,

            //Keywords
            AND, BREAK, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR, PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

            //Other
            EOF
        }

        public TokenType type;
        public string lexeme;
        public Object literal;
        public int line;

        public Token(TokenType tType, string lex, Object lit, int ln)
        {
            this.type = tType;
            lexeme = lex;
            literal = lit;
            line = ln;
        }

        public override string ToString()
        {
            return type + " " + lexeme?.ToString()??"" + " " + literal?.ToString()??"";
        }

    }
}
