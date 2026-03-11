using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Dependencies;

public abstract class Dependency
{
    private readonly IServiceProvider _services;
    
    private readonly IFileAccessService _files;
    
    public abstract string Name { get; }
    public abstract string FileName { get; }
    public abstract string Version { get; }
    public abstract string LatestVersion { get; }
    
    public bool IsInstalled => GetFile().Exists;

    protected Dependency(IServiceProvider services) {
        _services = services;
    }

    protected abstract Task<MemoryStream> DownloadInternalAsync();

    public FileInfo GetFile() => _files.GetDependencyFile(this);

    public async Task DownloadAsync() {
        var file = GetFile();
        
        var fs = file.OpenWrite();
        var ds = await DownloadInternalAsync();
        
        await ds.CopyToAsync(fs);
    }
}
