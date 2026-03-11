using System.Diagnostics;
using PamelloV7.Framework.Dependencies;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Module.Marsoau.Base.Dependencies;

public class YtDlpDependency : SimpleDependency
{
    public override string Name => "yt-dlp";
    public override string InternalFilePath => "yt-dlp";
    
    protected override string VersionProperty => "tag_name";
    protected override string VersionUrl => "https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest";
    protected override string DownloadUrl => "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_linux";

    public YtDlpDependency(IServiceProvider services) : base(services) { }
}
