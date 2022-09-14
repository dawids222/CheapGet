using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using System.Diagnostics;

namespace LibLite.CheapGet.DAL.Services
{
    public class FileService : IFileService, IResourceService
    {
        public Task<string> ReadAsync(string path)
        {
            return File.ReadAllTextAsync(path);
        }

        public Task SaveAsync(FileModel file)
        {
            Directory.CreateDirectory(file.Path);
            return File.WriteAllBytesAsync(file.FullPath, file.Content);
        }

        public void Delete(FileModel file)
        {
            File.Delete(file.FullPath);
        }

        public void Open(FileModel file)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(file.FullPath)
                {
                    UseShellExecute = true,
                },
            };
            process.Start();
        }

        public Task<string> ReadAllTextFromResourceAsync(string fullname)
        {
            return File.ReadAllTextAsync($"Resources\\{fullname}");
        }
    }
}
