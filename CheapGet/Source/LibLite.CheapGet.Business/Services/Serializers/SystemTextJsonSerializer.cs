using LibLite.CheapGet.Business.Services.Serializers.Converters;
using LibLite.CheapGet.Core.Services;
using System.Text.Json;

namespace LibLite.CheapGet.Business.Services.Serializers
{
    public class SystemTextJsonSerializer : ISerializer
    {
        private JsonSerializerOptions _options;

        public SystemTextJsonSerializer()
        {
            _options = new()
            {
                PropertyNameCaseInsensitive = true,
            };
            _options.Converters.Add(new CultureSpecificQuotedDecimalConverter());
        }

        public T Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, _options);
        public string Serialize(object value) => JsonSerializer.Serialize(value, _options);
    }
}
