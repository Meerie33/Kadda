using System;
using System.Collections.Generic;
using System.Linq;
using Kadda.CodeAnalysis;

namespace Kadda
{
    class Program
    {
        static void Main(string[] args)
        {
            bool showTree = false;
            while(true)
            {
                Console.Write(">> ");
                var line = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(line))
                    return;

                if(line == ">showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                    continue;
                }
                else if(line == ">cls")
                {
                    Console.Clear();
                    continue;
                }
                var parser = new Parser(line);
                var syntaxTree = SyntaxTree.Parse(line);

                if(showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ForegroundColor = color;
                }

                // Error found
                if(!syntaxTree.Diagnostics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var result = e.Evaluate();
                    Console.WriteLine(result);                 
                }
                else
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach(var diagnostics in parser.Diagnostics)
                        Console.WriteLine(diagnostics);
                    
                    Console.ForegroundColor = color; 
                }
            }
        }   

        static void PrettyPrint(SyntaxNode node, string indend = "", bool isLast = true)
        {
            // Unix Tree
            // 
            // └──
            // │ 
            // ├── 

            var marker = isLast ? "└──" : "├──";
            Console.Write(indend);
            Console.Write(marker);
            Console.Write(node.Kind);

            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write("");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indend += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach(var child in node.GetChildren())
                PrettyPrint(child, indend, child == lastChild);
        }
    }
}