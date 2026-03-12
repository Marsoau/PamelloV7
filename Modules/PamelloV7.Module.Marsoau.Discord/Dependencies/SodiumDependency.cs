using System.Reflection;
using Discord.WebSocket;
using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class SodiumDependency : MarsoauLibDependency
{
    public override string Name => "sodium";
    public override string InternalFilePath => "libsodium.so";
    protected override string[] DllNames => ["sodium", "libsodium"];

    public override Assembly DllAssembly => typeof(DiscordSocketClient).Assembly;

    public SodiumDependency(IServiceProvider services) : base(services) { }
}
