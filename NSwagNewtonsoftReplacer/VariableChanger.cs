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
        Dictionary<string, string> fieldsReplacementList = new Dictionary<string, string>
        {
            { "Newtonsoft.Json.JsonSerializerSettings", "JsonSerializerOptions" }
        };

        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var varType = node?.Type?.ToString()?.Replace("?", "");
            var variables = node?.Variables;
            foreach (var f in fieldsReplacementList)
            {
                if (varType.Contains(f.Key))
                {
                    varType = varType.Replace(f.Key, f.Value);
                }
            }
            if (varType != null && variables != null)
                return CreateVariable(variables.Value, varType);
            else
                return base.VisitVariableDeclaration(node);
        }

        VariableDeclarationSyntax CreateVariable(SeparatedSyntaxList<VariableDeclaratorSyntax> variables, string typeName)
        {
            return SyntaxFactory.VariableDeclaration(ParseTypeName(typeName), variables);
        }
    }
}
