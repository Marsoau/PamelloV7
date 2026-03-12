using System.Reflection;
using Discord.WebSocket;
using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class OpusDependency : DllDependency
{
    public override string Name => "opus";
    public override string InternalFilePath => "libopus.so";
    protected override string[] DllNames => ["opus", "libopus"];

    public override Assembly DllAssembly => typeof(DiscordSocketClient).Assembly;
    
    protected override string VersionProperty => "";
    protected override string VersionUrl => "";
    protected override string DownloadUrlLinux => "";
    protected override string DownloadUrlWindows => "";
    protected override bool IsExecutable => false;

    public OpusDependency(IServiceProvider services) : base(services) { }
}
