using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Dependencies;

public abstract class Dependency
{
    private readonly IServiceProvider _services;

    private readonly IFileAccessService _files;

    public abstract string Name { get; }
    public abstract string InternalFilePath { get; }

    public bool IsInstalled => GetFile().Exists;
    
    private Task? _downloadTask;

    protected Dependency(IServiceProvider services) {
        _services = services;

        _files = services.GetRequiredService<IFileAccessService>();
    }

    public DirectoryInfo GetDirectory() => _files.GetDependencyDirectory(this);
    public FileInfo GetFile() => _files.GetDependencyFile(this);
    
    public abstract Task<string> GetInstalledVersionAsync();
    public abstract Task<string> GetLatestVersionAsync();

    protected abstract Task DownloadOrUpdateInternalAsync(DirectoryInfo directory);

    public async Task DownloadOrUpdateAsync() {
        if (_downloadTask is not null) {
            await _downloadTask;
            return;
        }
        
        var directory = GetDirectory();
        
        _downloadTask = DownloadOrUpdateInternalAsync(directory); 
        
        try {
            await _downloadTask;
        }
        finally {
            _downloadTask = null;
        }
    }
}
