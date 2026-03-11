using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Dependencies;

public abstract class SimpleDependency : Dependency
{
    private readonly IHttpClientFactory _clientFactory;
    
    protected abstract string VersionProperty { get; }
    protected abstract string VersionUrl { get; }
    protected abstract string DownloadUrl { get; }

    protected SimpleDependency(IServiceProvider services) : base(services) {
        _clientFactory = services.GetRequiredService<IHttpClientFactory>();
    }

    public override async Task<string?> GetLatestVersionAsync() {
        using var client = _clientFactory.CreateClient();
        
        var json = await client.GetStringAsync(VersionUrl);
        using var doc = JsonDocument.Parse(json);
        
        return doc.RootElement.GetProperty(VersionProperty).GetString() ?? "";
    }

    protected override async Task DownloadOrUpdateInternalAsync(DirectoryInfo directory) {
        var client = _clientFactory.CreateClient();
        var file = GetFile();

        var fileBytes = await client.GetByteArrayAsync(DownloadUrl);
        await File.WriteAllBytesAsync(file.FullName, fileBytes);

        File.SetUnixFileMode(file.FullName, File.GetUnixFileMode(file.FullName) | UnixFileMode.UserExecute);
    }
}
