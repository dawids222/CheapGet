using LibLite.CheapGet.Core.Services;
using System.Text.Json;

namespace LibLite.CheapGet.Business.Services.Serializers
{
    public class SystemTextJsonSerializer : ISerializer
    {
        public T Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value);
        public string Serialize(object value) => JsonSerializer.Serialize(value);
    }
}
