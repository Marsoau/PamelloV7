using System.Diagnostics;
using PamelloV7.Framework.Dependencies;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Module.Marsoau.Base.Dependencies;

public class FFMpegDependency : SingleFileDependency
{
    public override string Name => "ffmpeg";
    public override string InternalFilePath => "ffmpeg";

    protected override string VersionProperty => "name";
    protected override string VersionUrl => "https://api.github.com/repos/shaka-project/static-ffmpeg-binaries/releases/latest";
    protected override string DownloadUrl => "https://github.com/shaka-project/static-ffmpeg-binaries/releases/latest/download/ffmpeg-linux-x64";
    protected override bool IsExecutable => true;

    public FFMpegDependency(IServiceProvider services) : base(services) { }
}
