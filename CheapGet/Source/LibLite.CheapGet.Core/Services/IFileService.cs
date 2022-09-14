using LibLite.CheapGet.Core.Services.Models;

namespace LibLite.CheapGet.Core.Services
{
    public interface IFileService
    {
        Task<string> ReadAsync(string path);
        Task SaveAsync(FileModel file);
        void Delete(FileModel file);
        void Open(FileModel file);
    }
}
