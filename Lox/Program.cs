using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    class Program
    {

        public static bool hasError = false;
        public static bool hadRuntimeError = false;

        private static Interpreter interpreter = new Interpreter();

        static void Main(string[] args)
        {

            if(args.Length > 1)
            {
                Console.WriteLine("Usage: clox [script]");
            }

            else if (args.Length == 1)
            {
                runFile(args[0]);
            }

            else
            {
                runPrompt();
            }
        }

        private static void runFile(string path)
        {
            StreamReader reader = new StreamReader(path);
            run(reader.ReadToEnd());
            if(hasError)
            {
                //add code for exiting application later
            }
            if (hadRuntimeError)
            {
                //add separate code for exiting application later
            }
            Console.ReadKey();
        }

        private static void runPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                run(Console.ReadLine());
            }
        }

        private static void run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            List<Stmnt.Stmt> statements = parser.parse();

            if (hasError) return;

            interpreter.interpret(statements);
        }

        public static void error(Token token, string message)
        {
            if(token.type == Token.TokenType.EOF)
            {
                report(token.line, "at end", message);
            }
            else
            {
                report(token.line, $"at {token.lexeme}", message);
            }
        }

        public static void runtimeError(RuntimeError error)
        {
            Console.WriteLine($"{error.Message} \n [line {error.token.line}]");
            hadRuntimeError = true;
        }

        public static void error(int line, string message)
        {
            report(line, "", message);
        }

        private static void report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where} : {message}");
            hasError = true;
        }
        
    }
}
