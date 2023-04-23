namespace LibLite.CheapGet.Core.Services
{
    public interface IEnvironmentService
    {
        Task ClearInputAsync();
        Task ExitApplicationAsync();
    }
}
