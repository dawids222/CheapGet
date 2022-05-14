namespace LibLite.CheapGet.Core.Services
{
    public interface IHttpClient
    {
        Task<TResponse> GetAsync<TResponse>(string url, CancellationToken token);
    }
}
