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
        Dictionary<string, string> propReplacementList = new Dictionary<string, string>
        {
            { "Newtonsoft.Json.JsonSerializerSettings", "JsonSerializerOptions" },
            {"Newtonsoft.Json.JsonConvert.SerializeObject", "JsonSerializer.Serialize" },

            { "Newtonsoft.Json.JsonProperty", "JsonPropertyName" },
            { ", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore", ""},
            { "Newtonsoft.Json.JsonExtensionData", "JsonExtensionData"},
            { "Newtonsoft.Json.JsonConverter", "JsonConverter" },
            { "Newtonsoft.Json.Converters.StringEnumConverter", "JsonStringEnumConverter"}
        };

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var prop = node?.ToString()?.Replace("?", "");

            foreach (var f in propReplacementList)
            {
                if (prop.Contains(f.Key))
                {
                    prop = prop.Replace(f.Key, f.Value);
                }
            }

            return ParseMemberDeclaration(prop);
            //return base.VisitPropertyDeclaration(node);
        }
    }
}
