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

    public async Task<bool> IsLatestAsync() {
        var installedVersion = GetInstalledVersionAsync();
        var latestVersion = GetLatestVersionAsync();
        
        return await installedVersion == await latestVersion;
    }

    public DirectoryInfo GetDirectory() => _files.GetDependencyDirectory(this);
    public FileInfo GetFile() => _files.GetDependencyFile(this);

    public FileInfo GetVersionFile() => new (
        Path.Combine(GetDirectory().FullName, "version.txt")
    );

    public async Task<string?> GetInstalledVersionAsync() {
        var file = GetVersionFile();
        if (!file.Exists) return null;
        
        var version = await File.ReadAllTextAsync(file.FullName);
        if (string.IsNullOrWhiteSpace(version)) return null;
        
        return version.Trim();
    }
    public abstract Task<string?> GetLatestVersionAsync();

    protected abstract Task DownloadOrUpdateInternalAsync(DirectoryInfo directory);

    public async Task DownloadOrUpdateAsync() {
        if (_downloadTask is not null) {
            await _downloadTask;
            return;
        }
        
        var directory = GetDirectory();
        
        _downloadTask = DownloadOrUpdateInternalAsync(directory);

        var version = await GetLatestVersionAsync();
        
        try {
            await _downloadTask;
        }
        finally {
            _downloadTask = null;
            await File.WriteAllTextAsync(GetVersionFile().FullName, version ?? "");
        }
    }
}
