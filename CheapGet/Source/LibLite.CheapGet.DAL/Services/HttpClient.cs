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

        public Task<string> GetStringAsync(string url, CancellationToken token)
        {
            return _client.GetStringAsync(url, token);
        }

        public Task<TResponse> GetAsync<TResponse>(string url, CancellationToken token)
        {
            return GetAsync<TResponse>(url, new Dictionary<string, string>(), token);
        }

        public async Task<TResponse> GetAsync<TResponse>(string url, IDictionary<string, string> headers, CancellationToken token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await _client.SendAsync(request, token);
            var value = await response.Content.ReadAsStringAsync(token);
            return _serializer.Deserialize<TResponse>(value);
        }
    }
}
