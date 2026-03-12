using System.Reflection;
using Discord.LibDave;
using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class DaveDependency : DllDependency
{
    public override string Name => "dave";
    public override string InternalFilePath => "libdave.so";
    protected override string[] DllNames => ["dave", "libdave"];
    
    public override Assembly DllAssembly => typeof(Dave).Assembly;

    protected override string VersionProperty => "";
    protected override string VersionUrl => "";
    protected override string DownloadUrl => "";

    public DaveDependency(IServiceProvider services) : base(services) { }
}
