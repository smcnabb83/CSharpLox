using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAST
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generateAst <output_directory>");
            }

            String outputDir = args[0];
            defineAst($"{outputDir}\\Stmt.cs", "Stmt", new List<string>() {"Expression: Expr expression",
                                                                           "Print : Expr expression",
                                                                           "Var: Token name, Expr initializer",
                                                                           "Block: List<Stmt> statements",
                                                                           "If: Expr condition, Stmt thenBranch, Stmt elseBranch",
                                                                           "While: Expr condition, Stmt body",
                                                                           "Break: Token breakToken",
                                                                           "Function: Token name, List<Token> parameters, List<Stmt> body",
                                                                           "Return: Token keyword, Expr value",
                                                                           "Class: Token name, Variable superclass, List<Function> methods",
                                                                           "Try: Stmt tryStmt, Stmt catchStmt"});

            defineAst($"{outputDir}\\Expr.cs", "Expr", new List<string>(){"Binary: Expr left, Token Operator, Expr right",
                                                                          "Grouping: Expr expression",
                                                                          "Literal: Object value",
                                                                          "Unary: Token Operator, Expr right",
                                                                          "Variable: Token name",
                                                                          "Assign: Token name, Expr value",
                                                                          "Logical: Expr left, Token Operator, Expr right",
                                                                          "Call: Expr callee, Token paren, List<Expr> Arguments",
                                                                          "Get: Expr Object, Token name",
                                                                          "Set: Expr Object, Token name, Expr value",
                                                                          "This: Token keyword",
                                                                          "Super: Token keyword, Token method"});
        }

        private static void defineAst(String outputDir, string baseName, List<string> types)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputDir, false))
            {
                file.WriteLine("using System;");
                file.WriteLine("using System.Collections.Generic;");
                file.WriteLine("using System.Linq;");
                file.WriteLine("using System.Text;");
                file.WriteLine("using System.Threading.Tasks;");
                file.WriteLine(" ");
                file.WriteLine($"namespace Lox.G{baseName}");
                file.WriteLine("{");

                defineVisitor(file, baseName, types);

                file.WriteLine($"public abstract class {baseName}");
                file.WriteLine("{");
                file.WriteLine("\tpublic abstract T Accept<T>(Visitor<T> visitor);");
                file.WriteLine("}");
                file.WriteLine(" ");

                foreach(string type in types)
                {
                    string classname = type.Split(':')[0].Trim();
                    string fields = type.Split(':')[1].Trim();
                    defineType(file, baseName, classname, fields);
                }
                file.WriteLine("}");
            }

        }

        private static void defineVisitor(System.IO.StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine("public interface Visitor<T> {");
            foreach(string type in types)
            {
                string typeName = type.Split(':')[0].Trim();
                writer.WriteLine($"T visit_{typeName}_{baseName} ({typeName} {baseName.ToLower()});");
            }
            writer.WriteLine("}");
        }

        private static void defineType(System.IO.StreamWriter writer, string basename, string classname, string fieldList)
        {

            List<string> fields = new List<string>();
            foreach(string s in fieldList.Split(','))
            {
                fields.Add(s.Trim());
            }

            writer.WriteLine($"public class {classname} : {basename}");
            writer.WriteLine("{");
            writer.WriteLine(" ");
            foreach(string field in fields)
            {
                writer.WriteLine($"\tpublic {field};");
            }
            writer.WriteLine(" ");
            writer.WriteLine($"\tpublic {classname} ({fieldList})");
            writer.WriteLine("\t{");
            foreach(string field in fields)
            {
                writer.WriteLine($"\t\tthis.{field.Split(' ')[1]} = {field.Split(' ')[1]};");
            }
            writer.WriteLine("\t}");
            writer.WriteLine(" ");
            writer.WriteLine("\toverride public T Accept<T>(Visitor <T> visitor) {");
            writer.WriteLine($"\t\treturn visitor.visit_{classname}_{basename}(this);");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

        }
    }
}
