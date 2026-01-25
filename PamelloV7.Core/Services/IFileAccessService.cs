using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IFileAccessService : IPamelloService
{
    public FileInfo? GetFile(string path);
    public FileInfo GetSourceFile(SongSource source);
    public string GetPublicUrl(string path);
}
