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
        Dictionary<string, ReplaceValue> attrReplacementList = new Dictionary<string, ReplaceValue>
        {
            { "Newtonsoft.Json.JsonProperty", new ReplaceValue("JsonPropertyName", (val) =>
            val.Replace(", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore", ""))},
            { "Newtonsoft.Json.JsonExtensionData", new ReplaceValue("JsonExtensionData", null)},
            { "Newtonsoft.Json.JsonConverter", new ReplaceValue("JsonConverter", (val) => 
            val.Replace("Newtonsoft.Json.Converters.StringEnumConverter", "JsonStringEnumConverter"))}
        };

        /// Visited for all AttributeListSyntax nodes
        /// The method replaces all PreviousAttribute attributes annotating a method by ReplacementAttribute attributes
        public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            foreach (var pattern in attrReplacementList)
            {         
                if (GotAttribute(node, pattern.Key))
                {
                    string attrVal = GetAttributeValue(node);
                    return CreateAttribute(pattern.Value, attrVal);
                }
            }
            return base.VisitAttributeList(node);
        }       

        bool GotAttribute(AttributeListSyntax node, string containsName)
        {
            if (node.Attributes.Any(
                   currentAttribute => currentAttribute.Name.GetText().ToString() == (containsName)))
            {
                return true;
            }
            else
                return false;
        }

        string GetAttributeValue(AttributeListSyntax node)
        {
            return node.Attributes.FirstOrDefault()?
                .ArgumentList?.Arguments.ToString();
                //.Replace("\"", "");
        }

        AttributeListSyntax CreateAttribute(ReplaceValue val, string oldAttr)
        {
            // Return an alternate node that is injected instead of the current node
            if (oldAttr != null)
                return AttributeList(
                    SingletonSeparatedList(
                                Attribute(IdentifierName(val.Property),
                                    AttributeArgumentList(
                                        SeparatedList(new[]
                                        {
                                    AttributeArgument(
                                       ParseExpression(val.ValueReplacer(oldAttr))
                                        )
                                        })))));
            else
                return AttributeList(
                            SingletonSeparatedList(
                            Attribute(IdentifierName(val.Property))));
        }
    }
}
