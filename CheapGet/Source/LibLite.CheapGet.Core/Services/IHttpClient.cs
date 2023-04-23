namespace LibLite.CheapGet.Core.Services
{
    public interface IHttpClient
    {
        Task<String> GetStringAsync(string url, CancellationToken token);
        Task<TResponse> GetAsync<TResponse>(string url, CancellationToken token);
        Task<TResponse> GetAsync<TResponse>(string url, IDictionary<string, string> headers, CancellationToken token);
    }
}
