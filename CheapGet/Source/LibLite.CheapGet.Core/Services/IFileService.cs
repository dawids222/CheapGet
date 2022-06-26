using LibLite.CheapGet.Core.Services.Models;

namespace LibLite.CheapGet.Core.Services
{
    public interface IFileService
    {
        Task SaveAsync(FileModel file);
        void Delete(FileModel file);
        void Open(FileModel file);
    }
}
