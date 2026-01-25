using PamelloV7.Core.Services;
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
        => GetFile(requestedPath) ?? throw new FileNotFoundException();
    public FileInfo? GetFile(string requestedPath) {
        if (requestedPath.Split('/').Any(part => part == "..")) {
            return null;
        }
        
        return new FileInfo($"{_root}{requestedPath}");
    }

    public string GetPublicUrl(string path) {
        return $"https://{ServerConfig.Root.HostName}/Files{path}";
    }
}
