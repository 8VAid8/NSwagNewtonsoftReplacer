//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

//namespace NSwagNewtonsoftReplacer
//{
//    public partial class CodeChanger : CSharpSyntaxRewriter
//    {
//        Dictionary<string, string> invReplacementList = new Dictionary<string, string>
//        {
//            { "Newtonsoft.Json.JsonConvert.DeserializeObject", "JsonSerializer.Deserialize" }
//        };

//        public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
//        {
//            return base.VisitAnonymousMethodExpression(node);
//        }
//        {
//            string newExpression = null;
//            foreach (var i in invReplacementList)
//            {
//                if (node.ToString().Contains(i.Key))
//                {
//                    newExpression = node.ToString().Replace(i.Key, i.Value);
//                }
//            }
//            if (newExpression != null)
//            {
//                return ParseExpression(newExpression);
//            }
//            else
//                return base.VisitInvocationExpression(node);
//        }
//    }
//}
