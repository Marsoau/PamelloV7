using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public interface IFileAccessService : IPamelloService
{
    public FileInfo? GetFile(string path);
    public FileInfo GetSourceFile(SongSource source);
    public string GetPublicUrl(string path);
}
