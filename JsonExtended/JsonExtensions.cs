using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

namespace JsonExtended
{
    public static class JsonExtensions
    {
        public static T Deserialize<T>(this byte[] data, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(data.ToText(), options);
        }

        public static T Deserialize<T>(this Stream stream, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(stream.ToText(), options);
        }

        public static T FromJson<T>(this string jsonText, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(jsonText, options);
        }

        public static dynamic ToDynamic(this string jsonText)
        {
            return JsonSerializer.Deserialize<dynamic>(jsonText);
        }

        public static string ToIndentedJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
        public static string ToFormattedJson(this string jsonText)
        {
            jsonText = jsonText.Trim().Trim(',');
            var parsedJson = JsonDocument.Parse(jsonText, new JsonDocumentOptions() { AllowTrailingCommas = true });
            var result = JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions { WriteIndented = true });
            return result;
        }

        public static JsonElement ToJsonElement(this JsonNode jsonNode)
        {
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(jsonNode);
            return element;
        }

        public static IEnumerable<string> GetPaths(this JsonDocument jsonDocument)
        {
            var jsonElement = jsonDocument.RootElement;
            var result = jsonElement.GetKeys().Select(x => x.Substring(0, x.LastIndexOfAny(new[] { '-' })));
            return result;
        }

        public static IEnumerable<string> GetPaths(this JsonElement jsonElement)
        {
            var result = jsonElement.GetKeys().Select(x => x.Substring(0, x.LastIndexOfAny(new[] { '-' })));
            return result;
        }

        public static IEnumerable<string> GetKeys(this JsonDocument jsonDocument)
        {
            var jsonElement = jsonDocument.RootElement;
            return jsonElement.GetKeys();
        }
        public static IEnumerable<string> GetKeys(this JsonElement jsonElement)
        {
            var queu = new Queue<(string ParentPath, JsonElement element)>();
            queu.Enqueue(("", jsonElement));
            while (queu.Any())
            {
                var (parentPath, element) = queu.Dequeue();
                switch (element.ValueKind)
                {
                    case JsonValueKind.Object:
                        parentPath = parentPath == ""
                            ? "$."
                            : parentPath + ".";
                        foreach (var nextEl in element.EnumerateObject())
                        {
                            queu.Enqueue(($"{parentPath}{nextEl.Name}", nextEl.Value));
                        }
                        yield return parentPath.Trim('.') + "-object";
                        break;
                    case JsonValueKind.Array:
                        foreach (var (nextEl, i) in element.EnumerateArray().Select((jsonElement, i) => (jsonElement, i)))
                        {
                            if (string.IsNullOrEmpty(parentPath))
                                parentPath = "$.";
                            queu.Enqueue(($"{parentPath}[{i}]", nextEl));
                        }
                        yield return parentPath.Trim('.') + "-array"; ;
                        break;
                    case JsonValueKind.String:
                        var isDate = DateTime.TryParse(element.ToString(), out _);
                        var type = isDate ? "-date" : "-string";
                        yield return parentPath.Trim('.') + type;
                        break;
                    case JsonValueKind.Number:
                        yield return parentPath.Trim('.') + "-number"; ;
                        break;
                    case JsonValueKind.Undefined:
                        yield return parentPath.Trim('.') + "-undefined";
                        break;
                    case JsonValueKind.Null:
                        yield return parentPath.Trim('.') + "-null";
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        yield return parentPath.Trim('.') + "-boolean"; ;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static bool? IsSwaggerJson(string swaggerJsonText)
        {
            try
            {
                var parsedJson = JsonNode.Parse(swaggerJsonText);
                var paths = parsedJson["paths"];
                var openapi = parsedJson["openapi"];
                var swagger = parsedJson["swagger"];
                if (paths != null && (openapi != null || swagger != null))
                    return true;

                return false;
            }
            catch
            {
                return null;
            }
        }

        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(obj, options);
        }

        private static string ToText(this byte[] bytes)
        {
            return Encoding.UTF32.GetString(bytes);
        }

        private static string ToText(this Stream @this)
        {
            using (var sr = new StreamReader(@this, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }


    }
}
