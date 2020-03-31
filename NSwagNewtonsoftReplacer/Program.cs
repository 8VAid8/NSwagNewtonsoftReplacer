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
            Regex JsonPropertyPattern = new Regex(@"^ *\[Newtonsoft\.Json\.JsonProperty.*$");
            Regex JsonPropertyNamePattern = new Regex(@"(?<=\[Newtonsoft\.Json\.JsonProperty\("").*(?=""\, Required)");

            foreach (var arg in args)
            {
                string filePath = arg;
                var lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("namespace"))
                    {
                        lines[i + 1] += "\n    using System.Text.Json.Serialization;\n    using System.Text.Json;";
                    }

                    var property = JsonPropertyNamePattern.Match(lines[i]);
                    if (property.Success)
                    {
                        lines[i] = "        [JsonPropertyName(\"" + property.Value + "\")]";
                    }

                    if (lines[i].Contains("[Newtonsoft.Json.JsonExtensionData]"))
                    {
                        lines[i] = lines[i].Replace("Newtonsoft.Json.", "");
                    }

                    if (lines[i].Contains("[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]"))
                    {
                        lines[i] = lines[i].Replace("Newtonsoft.Json.", "")
                            .Replace("Converters.StringEnumConverter", "JsonStringEnumConverter");
                    }

                    if (lines[i].Contains("Newtonsoft.Json.JsonSerializerSettings"))
                    {
                        lines[i] = lines[i].Replace("Newtonsoft.Json.JsonSerializerSettings", "JsonSerializerOptions");
                    }

                    if (lines[i].Contains("Newtonsoft.Json.JsonConvert.SerializeObject"))
                    {
                        lines[i] = lines[i].Replace("Newtonsoft.Json.JsonConvert.SerializeObject", "JsonSerializer.Serialize");
                    }

                    if (lines[i].Contains("Newtonsoft.Json.JsonConvert.DeserializeObject"))
                    {
                        lines[i] = lines[i].Replace("Newtonsoft.Json.JsonConvert.DeserializeObject", "JsonSerializer.Deserialize");
                    }

                    if (lines[i].Contains("Newtonsoft.Json.JsonException"))
                    {
                        lines[i] = lines[i].Replace("Newtonsoft.Json.", "");
                    }

                    if (lines[i].Contains("using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))"))
                    {
                        lines[i] = "                        var responseBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);";
                        lines[i + 1] = lines[i + 2] = lines[i + 3] = lines[i + 4] = lines[i + 7] = string.Empty;
                        lines[i + 5] = lines[i + 5].Replace("serializer", "JsonSerializer")
                            .Replace("jsonTextReader", "responseBytes");
                    }
                }

                File.WriteAllLines(filePath, lines);
            }
            Console.WriteLine("Conversion succeeded!");
        }
    }
}
