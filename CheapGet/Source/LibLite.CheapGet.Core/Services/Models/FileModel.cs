namespace LibLite.CheapGet.Core.Services.Models
{
    public class FileModel
    {
        public string Path { get; init; }
        public string Name { get; init; }
        public string Extension { get; init; }
        public byte[] Content { get; init; }

        public string FullPath => $"{Path}\\{FullName}";
        public string FullName => $"{Name}.{Extension}";

        public FileModel() { }
        public FileModel(string path, string name, string extension, byte[] content)
        {
            Path = path;
            Name = name;
            Extension = extension;
            Content = content;
        }
    }
}
