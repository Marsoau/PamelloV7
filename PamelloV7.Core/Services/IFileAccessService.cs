using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IFileAccessService : IPamelloService
{
    public FileInfo? GetFile(string path);
    public string GetPublicUrl(string path);
}
