using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
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
            {"Newtonsoft.Json.JsonConvert.SerializeObject", "JsonSerializer.Serialize" },
            { "Newtonsoft.Json.JsonException", "JsonException" }
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
            StringBuilder sb = new StringBuilder();
            using (StringReader r = new StringReader(node.ToString()))
            {
                string line;
                do
                {
                    line = r.ReadLine();

                    if (line.Contains("using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))"))
                    {
                        line = "                        var responseBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);";
                        sb.AppendLine(line);
                        line = r.ReadLine(); line = String.Empty; sb.AppendLine(line);
                        line = r.ReadLine(); line = String.Empty; sb.AppendLine(line);
                        line = r.ReadLine(); line = String.Empty; sb.AppendLine(line);
                        line = r.ReadLine(); line = String.Empty; sb.AppendLine(line);
                        line = r.ReadLine();
                        line = line.Replace("serializer", "JsonSerializer").Replace("jsonTextReader", "responseBytes");
                        sb.AppendLine(line);
                        line = r.ReadLine(); sb.AppendLine(line);
                        line = r.ReadLine(); line = string.Empty; sb.AppendLine(line);
                    }
                    else if (line.Contains("settings = new Newtonsoft.Json.JsonSerializerSettings") ||
                        line.Contains("settings = new JsonSerializerOptions"))
                    {
                        sb.AppendLine(line);
                        sb.AppendLine("settings.Converters.Add(new JsonStringEnumConverter());");      
                    }
                    else
                        sb.AppendLine(line);
                }
                while (r.Peek() > 0);      
            }

            string newDeclaration = sb.Length > 0 ? sb.ToString() : null;
            foreach (var m in methodReplacementList)
            {
                if (node.ToString().Contains(m.Key))
                {
                    newDeclaration = newDeclaration.Replace(m.Key, m.Value);
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
