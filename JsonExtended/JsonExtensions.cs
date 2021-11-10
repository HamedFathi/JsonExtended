using System.IO;
using System.Text;
using System.Text.Json;

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
