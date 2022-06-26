namespace LibLite.CheapGet.Core.Services
{
    public interface IResourceService
    {
        Task<string> ReadAllTextFromResourceAsync(string fullname);
    }
}
