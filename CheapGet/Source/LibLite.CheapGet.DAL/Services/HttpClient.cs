using LibLite.CheapGet.Core.Services;

namespace LibLite.CheapGet.DAL.Services
{
    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _client;
        private readonly ISerializer _serializer;

        public HttpClient(
            System.Net.Http.HttpClient client,
            ISerializer serializer)
        {
            _client = client;
            _serializer = serializer;
        }

        public async Task<TResponse> GetAsync<TResponse>(string url, CancellationToken token)
        {
            var response = await _client.GetStringAsync(url, token);
            return _serializer.Deserialize<TResponse>(response);
        }
    }
}
