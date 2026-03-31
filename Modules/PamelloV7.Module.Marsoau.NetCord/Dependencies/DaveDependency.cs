using System.Reflection;
using NetCord.Gateway;
using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.NetCord.Dependencies;

public class DaveDependency : MarsoauLibDependency
{
    public override string Name => "dave";
    public override string InternalFilePath => "libdave.so";
    protected override string[] DllNames => ["dave", "libdave"];
    
    public override Assembly DllAssembly => typeof(GatewayClient).Assembly;

    public DaveDependency(IServiceProvider services) : base(services) { }
}
