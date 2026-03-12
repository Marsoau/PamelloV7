using System.Reflection;
using Discord.WebSocket;
using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class OpusDependency : MarsoauLibDependency
{
    public override string Name => "opus";
    public override string InternalFilePath => "libopus.so";
    protected override string[] DllNames => ["opus", "libopus"];

    public override Assembly DllAssembly => typeof(DiscordSocketClient).Assembly;

    public OpusDependency(IServiceProvider services) : base(services) { }
}
