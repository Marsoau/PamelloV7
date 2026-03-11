using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public interface IFileAccessService : IPamelloService
{
    public FileInfo? GetFile(string path);
    public FileInfo GetSourceFile(SongSource source);
    public FileInfo GetDependencyFile(Dependency dependency);
    public DirectoryInfo GetDependencyDirectory(Dependency dependency);
    public string GetPublicUrl(string path);
}
