#nullable enable
#if NET6_0_OR_GREATER

using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace JsonExtended
{
    // var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    // options.Converters.Add(new DateOnlyConverter());
    // options.Converters.Add(new TimeOnlyConverter());

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private readonly string serializationFormat;

        public TimeOnlyConverter() : this(null)
        {
        }

        public TimeOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
        }

        public override TimeOnly Read(ref Utf8JsonReader reader,
                                Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return TimeOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value,
                                            JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(serializationFormat));
    }
}

#endif