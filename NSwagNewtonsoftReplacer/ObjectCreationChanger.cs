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
        Dictionary<string, string> creationReplacementList = new Dictionary<string, string>
        {
            { "Newtonsoft.Json.JsonSerializerSettings", "JsonSerializerOptions" },
            {"Newtonsoft.Json.JsonConvert.SerializeObject", "JsonSerializer.Serialize" }
        };

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            string newNodeType = node.Type.ToString();
            foreach (var c in creationReplacementList)
            {
                if (node.Type.ToString().Contains(c.Key))
                {
                    newNodeType = node?.Type?.ToString().Replace(c.Key, c.Value);
                }
            }
            return CreateObjectCreationExpression(newNodeType, node.ArgumentList, node.Initializer);
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            string newDeclaration = node.ToString();
            foreach (var c in creationReplacementList)
            {
                if (node.ToString().Contains(c.Key))
                {
                    newDeclaration = node.ToString().Replace(c.Key, c.Value);    
                }
            }
            return ParseStatement(newDeclaration);
        }

        ObjectCreationExpressionSyntax CreateObjectCreationExpression(string typeName, ArgumentListSyntax arguments,
            InitializerExpressionSyntax initializer)
        {
            return ObjectCreationExpression(ParseTypeName(typeName), arguments, initializer);
        }
    }
}
