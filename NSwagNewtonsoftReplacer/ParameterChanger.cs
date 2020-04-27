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
        public override SyntaxNode VisitParameter(ParameterSyntax node)
        {
            var paramName = node.Identifier.Text;
            var paramType = node?.Type?.ToString()?.Replace("?", "");
            if (paramType != null)
                return CreateParameter(paramName, paramType);
            else
                return base.VisitParameter(node);
        }

        ParameterSyntax CreateParameter(string name, string typeName)
        {
            return Parameter(Identifier(name)).WithType(ParseTypeName(typeName));
        }
    }
}
