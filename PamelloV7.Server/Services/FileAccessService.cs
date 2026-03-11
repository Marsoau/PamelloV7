using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Config;

namespace PamelloV7.Server.Services;

public class FileAccessService : IFileAccessService
{
    private readonly IServiceProvider _services;
    
    private readonly string _root;

    public FileAccessService(IServiceProvider services) {
        _services = services;
        
        _root = Path.Combine(ServerConfig.Root.DataPath, "Files");
    }

    public FileInfo GetFileRequired(string requestedPath)
        => GetFile(requestedPath) ?? throw new PamelloException("Unreacheable file");
    public FileInfo? GetFile(string requestedPath) {
        if (requestedPath.Split('/').Any(part => part == "..")) {
            return null;
        }
        
        return new FileInfo($"{_root}{requestedPath}");
    }

    public FileInfo GetSourceFile(SongSource source) {
        return GetFileRequired($"/Audio/{source.Song.Id}-{source.PK}.opus");
    }

    public FileInfo GetDependencyFile(Dependency dependency) {
        var dependencyDirectory = GetDependencyDirectory(dependency);
        
        return new FileInfo(
            Path.Combine(dependencyDirectory.FullName, dependency.InternalFilePath)
        );
    }

    public DirectoryInfo GetDependencyDirectory(Dependency dependency) {
        var dependenciesDirectory = new DirectoryInfo(
            Path.Combine(AppContext.BaseDirectory, "Dependencies")
        );
        if (!dependenciesDirectory.Exists) dependenciesDirectory.Create();
        
        var dependencyDirectory = new DirectoryInfo(
            Path.Combine(dependenciesDirectory.FullName, dependency.Name)
        );
        if (!dependencyDirectory.Exists) dependencyDirectory.Create();
        
        return dependencyDirectory;
    }

    public string GetPublicUrl(string path) {
        return $"https://{ServerConfig.Root.HostName}/Files{path}";
    }
}
