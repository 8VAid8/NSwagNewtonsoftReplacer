using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

            // Generates the syntax rewriter
            var rewriter = new AttributeStatementChanger();
            root = rewriter.Visit(root);

            // Exchanges the document in the solution by the newly generated document
            using (TextWriter writer = new StreamWriter(filePath))
            {
                root.WriteTo(writer);
            }
        }
    }
}
