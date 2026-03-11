using PamelloV7.Framework.Dependencies;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Module.Marsoau.Base.Dependencies;

public class YtDlpDependency : Dependency
{
    private readonly IHttpClientFactory _clientFactory;
    
    public override string Name => "yt-dlp";
    public override string InternalFilePath => "yt-dlp";
    
    private const string VersionUrl = "https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest";
    private const string DownloadUrl = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_linux";

    public YtDlpDependency(IServiceProvider services) : base(services) {
        _clientFactory = services.GetRequiredService<IHttpClientFactory>();
    }

    public override Task<string> GetInstalledVersionAsync() {
        throw new NotImplementedException();
    }
    
    public override async Task<string> GetLatestVersionAsync() {
        using var client = _clientFactory.CreateClient();
        
        var json = await client.GetStringAsync(VersionUrl);
        using var doc = JsonDocument.Parse(json);
        
        return doc.RootElement.GetProperty("tag_name").GetString() ?? "";
    }

    protected override async Task DownloadOrUpdateInternalAsync(DirectoryInfo directory) {
        var client = _clientFactory.CreateClient();
        var file = GetFile();
        
        var fileBytes = await client.GetByteArrayAsync(DownloadUrl);
        await File.WriteAllBytesAsync(file.FullName, fileBytes);

        File.SetUnixFileMode(file.FullName, File.GetUnixFileMode(file.FullName) | UnixFileMode.UserExecute);
    }
}
