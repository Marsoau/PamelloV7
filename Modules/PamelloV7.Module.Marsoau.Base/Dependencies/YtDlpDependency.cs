using System.Diagnostics;
using PamelloV7.Framework.Dependencies;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Module.Marsoau.Base.Dependencies;

public class YtDlpDependency : SingleFileDependency
{
    public override string Name => "yt-dlp";
    public override string InternalFilePath => "yt-dlp";
    
    protected override string VersionProperty => "tag_name";
    protected override string VersionUrl => "https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest";
    protected override string DownloadUrlLinux => "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_linux";
    protected override string DownloadUrlWindows => "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_x86.exe";
    protected override bool IsExecutable => true;

    public YtDlpDependency(IServiceProvider services) : base(services) { }
}
