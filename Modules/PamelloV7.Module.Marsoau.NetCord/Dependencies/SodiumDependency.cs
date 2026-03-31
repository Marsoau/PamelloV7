using System.Reflection;
using NetCord.Gateway;
using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.NetCord.Dependencies;

public class SodiumDependency : MarsoauLibDependency
{
    public override string Name => "sodium";
    public override string InternalFilePath => "libsodium.so";
    protected override string[] DllNames => ["sodium", "libsodium"];

    public override Assembly DllAssembly => typeof(GatewayClient).Assembly;

    public SodiumDependency(IServiceProvider services) : base(services) { }
}
