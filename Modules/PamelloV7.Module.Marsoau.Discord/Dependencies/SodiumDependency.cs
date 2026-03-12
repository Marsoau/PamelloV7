using System.Reflection;
using Discord.WebSocket;
using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class SodiumDependency : DllDependency
{
    public override string Name => "sodium";
    public override string InternalFilePath => "libsodium.so";
    protected override string[] DllNames => ["sodium", "libsodium"];

    public override Assembly DllAssembly => typeof(DiscordSocketClient).Assembly;

    protected override string VersionProperty => "";
    protected override string VersionUrl => "";
    protected override string DownloadUrlLinux => "";
    protected override string DownloadUrlWindows => "";
    protected override bool IsExecutable => false;

    public SodiumDependency(IServiceProvider services) : base(services) { }
}
