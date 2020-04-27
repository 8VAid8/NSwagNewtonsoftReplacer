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
        Dictionary<string, string> methodReplacementList = new Dictionary<string, string>
        {
            { "Newtonsoft.Json.JsonSerializerSettings", "JsonSerializerOptions" },
            { "Newtonsoft.Json.JsonConvert.DeserializeObject", "JsonSerializer.Deserialize" },
        };
  
        public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
        {
            foreach (var m in methodReplacementList)
            {
                if (node.ToString().Contains(m.Key))
                {
                    var right = m.Value;
                    return IdentifierName(right); // TODo: change it
                }
            }
            return base.VisitQualifiedName(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            string newDeclaration = null;
            foreach (var m in methodReplacementList)
            {
                if (node.ToString().Contains(m.Key))
                {
                    newDeclaration = node.ToString().Replace(m.Key, m.Value);
                }
            }
            if (newDeclaration != null)
            {
                return ParseMemberDeclaration(newDeclaration);
            }
            else
                return base.VisitMethodDeclaration(node);
        }
    }
}
