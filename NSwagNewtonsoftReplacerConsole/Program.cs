using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// How to use: run dll with NSwag generated file paths as parameters, e.g.
/// NSwagNewtonsoftReplacer WebClient.cs WebClient.Contracts.cs
/// </summary>
namespace NSwagNewtonsoftReplacer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                string filePath = arg;
                CodeEditor.RepalaceNewton(filePath);   
            }
            Console.WriteLine("Conversion succeeded!");
        }
    }
}
