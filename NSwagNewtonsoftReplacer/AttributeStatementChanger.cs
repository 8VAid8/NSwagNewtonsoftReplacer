using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSwagNewtonsoftReplacer
{
    /// <summary>
    /// The CSharpSyntaxRewriter allows to rewrite the Syntax of a node
    /// </summary>
    public class AttributeStatementChanger : CSharpSyntaxRewriter
    {
        Regex JsonPropertyPattern = new Regex(@"^ *\[Newtonsoft\.Json\.JsonProperty.*$");
        Regex JsonPropertyNamePattern = new Regex(@"(?<=\[Newtonsoft\.Json\.JsonProperty\("").*(?=""\, Required)");

        Dictionary<string, ReplaceValue> replacementList = new Dictionary<string, ReplaceValue>
        {
            { "Newtonsoft.Json.JsonProperty", new ReplaceValue("JsonPropertyName", (val) =>
            val.Replace(", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore", ""))},
            { "Newtonsoft.Json.JsonExtensionData", new ReplaceValue("JsonExtensionData", null)},
            { "Newtonsoft.Json.JsonConverter", new ReplaceValue("JsonConverter", (val) => 
            val.Replace("Newtonsoft.Json.Converters.StringEnumConverter", "JsonStringEnumConverter"))},
            { "Newtonsoft.Json.JsonSerializerSettings", new ReplaceValue("JsonSerializerOptions", null) }, // field
            { "Newtonsoft.Json.JsonConvert.SerializeObject", new ReplaceValue("JsonSerializer.Serialize", null) }, //in method
            { "Newtonsoft.Json.JsonConvert.DeserializeObject", new ReplaceValue("JsonSerializer.Deserialize", null) }, // in method
            { "Newtonsoft.Json.JsonException", new ReplaceValue("JsonException", null) }, //in method
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
            //{ "", new ReplaceValue("", null) },
        };

        class ReplaceValue
        {
            public string Property { get; set; }
            public Func<string, string> ValueReplacer { get; set; }

            public ReplaceValue(string property, Func<string, string> valueReplacer)
            {
                Property = property;
                ValueReplacer = valueReplacer;
            }
        }

        /// Visited for all AttributeListSyntax nodes
        /// The method replaces all PreviousAttribute attributes annotating a method by ReplacementAttribute attributes
        public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            foreach (var pattern in replacementList)
            {         
                if (GotAttribute(node, pattern.Key))
                {
                    string attrVal = GetAttributeValue(node);
                    return ReturnNewAttribute(pattern.Value, attrVal);
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

        AttributeListSyntax ReturnNewAttribute(ReplaceValue val, string oldAttr)
        {
            // Return an alternate node that is injected instead of the current node
            if (oldAttr != null)
                return SyntaxFactory.AttributeList(
                                SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(val.Property),
                                    SyntaxFactory.AttributeArgumentList(
                                        SyntaxFactory.SeparatedList(new[]
                                        {
                                    SyntaxFactory.AttributeArgument(
                                       SyntaxFactory.ParseExpression(val.ValueReplacer(oldAttr))
                                        )
                                        })))));
            else
                return SyntaxFactory.AttributeList(
                            SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(val.Property))));
        }

        //string GetAttribute(AttributeListSyntax node, string containsName)
        //{
        //    if (/*node.Parent is MethodDeclarationSyntax &&*/
        //       // and if the attribute name is PreviousAttribute
        //       node.Attributes.Any(
        //           currentAttribute => currentAttribute.Name.GetText().ToString() == (containsName)))
        //    {
        //        // get first val
        //        return node.Attributes.Select(currentAttribute => 
        //        currentAttribute.ArgumentList.Arguments.FirstOrDefault()
        //        .Expression.GetFirstToken().ValueText).FirstOrDefault();
        //    }
        //    else
        //        return null;
        //}
        //AttributeListSyntax ReturnNewAttribute(string name, string val = null)
        //{
        //    // Return an alternate node that is injected instead of the current node
        //    if (val != null)
        //    return SyntaxFactory.AttributeList(
        //                    SyntaxFactory.SingletonSeparatedList(
        //                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(name),
        //                        SyntaxFactory.AttributeArgumentList(
        //                            SyntaxFactory.SeparatedList(new[]
        //                            {
        //                            SyntaxFactory.AttributeArgument(
        //                                SyntaxFactory.LiteralExpression(
        //                                    SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(val))
        //                                )
        //                            })))));
        //    else
        //        return SyntaxFactory.AttributeList(
        //                    SyntaxFactory.SingletonSeparatedList(
        //                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(name))));
        //}
    }
}
