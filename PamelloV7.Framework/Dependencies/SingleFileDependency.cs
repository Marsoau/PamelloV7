using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Dependencies;

public abstract class SingleFileDependency : Dependency
{
    private readonly IHttpClientFactory _clientFactory;
    
    protected abstract string VersionProperty { get; }
    protected abstract string VersionUrl { get; }
    
    protected abstract string DownloadUrlLinux { get; }
    protected abstract string DownloadUrlWindows { get; }

    protected SingleFileDependency(IServiceProvider services) : base(services) {
        _clientFactory = services.GetRequiredService<IHttpClientFactory>();
    }

    public override async Task<string?> GetLatestVersionAsync() {
        if (string.IsNullOrWhiteSpace(VersionUrl)) return null;
        
        using var client = _clientFactory.CreateClient();
        
        var json = await client.GetStringAsync(VersionUrl);
        
        try {
            using var doc = JsonDocument.Parse(json);
            
            var version = doc.RootElement.GetProperty(VersionProperty).GetString();
            return string.IsNullOrWhiteSpace(version) ? null : version;
        }
        catch {
            return null;
        }
    }

    protected override async Task DownloadOrUpdateInternalAsync(DirectoryInfo directory) {
        if (string.IsNullOrWhiteSpace(DownloadUrlLinux)) return;
        
        var client = _clientFactory.CreateClient();
        var file = GetFile();

        string url;
        if (OperatingSystem.IsWindows()) {
            url = DownloadUrlWindows;
        }
        else if (OperatingSystem.IsLinux()) {
            url = DownloadUrlLinux;
        }
        else return;

        var fileBytes = await client.GetByteArrayAsync(url);
        await File.WriteAllBytesAsync(file.FullName, fileBytes);

        if (IsExecutable && OperatingSystem.IsLinux()) {
            File.SetUnixFileMode(file.FullName, File.GetUnixFileMode(file.FullName) | UnixFileMode.UserExecute);
        }
    }
}
