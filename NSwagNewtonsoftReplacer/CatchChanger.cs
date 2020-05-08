using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSwagNewtonsoftReplacer
{
    /// <summary>
    /// The CSharpSyntaxRewriter allows to rewrite the Syntax of a node
    /// </summary>
    public partial class CodeChanger : CSharpSyntaxRewriter
    {
        Dictionary<string, string> catchReplacementList = new Dictionary<string, string>
        {
            { "Newtonsoft.Json.JsonException", "JsonException" }
        };

        public override SyntaxNode VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            string newCatch = null;
            foreach (var c in catchReplacementList)
            {
                if (node.Type.ToString().Contains(c.Key))
                {
                    newCatch = node.Type.ToString().Replace(c.Key, c.Value);
                }
            }
            if (newCatch != null)
            {
                return CatchDeclaration(ParseTypeName(newCatch),node.Identifier);
            }
            else
                return base.VisitCatchDeclaration(node);
        }
    }
}
