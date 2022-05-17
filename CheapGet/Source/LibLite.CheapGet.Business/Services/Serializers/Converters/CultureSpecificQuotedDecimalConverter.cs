using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibLite.CheapGet.Business.Services.Serializers.Converters
{
    internal class CultureSpecificQuotedDecimalConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString().Replace(".", ",");
                return double.Parse(value);
            }
            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
