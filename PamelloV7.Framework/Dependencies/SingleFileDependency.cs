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
    protected abstract string DownloadUrlMacOs { get; }

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

    protected override async Task DownloadOrUpdateInternalAsync(DirectoryInfo directory, Action<int, int>? progressCallback) {
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
        else if (OperatingSystem.IsMacOS()) {
            url = DownloadUrlMacOs;
        }
        else return;
        
        const int maxProgress = 100;

        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;

        await using var httpStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true);

        var buffer = new byte[81920];
        var totalRead = 0;
        int bytesRead;
        
        while ((bytesRead = await httpStream.ReadAsync(buffer)) > 0) {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            
            totalRead += bytesRead;
            
            progressCallback?.Invoke((int)((float)totalRead / totalBytes * maxProgress), maxProgress);
        }

        if (IsExecutable && (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())) {
            File.SetUnixFileMode(file.FullName, File.GetUnixFileMode(file.FullName) | UnixFileMode.UserExecute);
        }
    }
}
