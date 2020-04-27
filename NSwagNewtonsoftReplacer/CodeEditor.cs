using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.Workspaces;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSwagNewtonsoftReplacer
{
    class CodeEditor
    {
        /// The method calling the Syntax Rewriter
        public static void RepalaceNewton(string filePath)
        {
            var text = File.ReadAllText(filePath);
            // Selects the syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();

            // add usings
            var compilationUnitSyntax = (CompilationUnitSyntax)(root);
            var json = QualifiedName(IdentifierName("System.Text"), IdentifierName("Json"));
            var jsonSerialization = QualifiedName(IdentifierName("System.Text.Json"), IdentifierName("Serialization"));
            compilationUnitSyntax = compilationUnitSyntax.AddUsings(
                UsingDirective(json).NormalizeWhitespace(),
                UsingDirective(jsonSerialization).NormalizeWhitespace());
            root = compilationUnitSyntax;

            // Generates the syntax rewriter
            var rewriter = new CodeChanger();
            root = rewriter.Visit(root);

            // format document
            root = Formatter.Format(root, new AdhocWorkspace());

            // Exchanges the document in the solution by the newly generated document
            using (TextWriter writer = new StreamWriter(filePath))
            {
                root.WriteTo(writer);
            }
        }
    }
}
