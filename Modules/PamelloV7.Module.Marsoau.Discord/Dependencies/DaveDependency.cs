using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class DaveDependency : SimpleDependency
{
    public override string Name => "dave";
    public override string InternalFilePath => "libdave.so";

    protected override string VersionProperty => "";
    protected override string VersionUrl => "";
    protected override string DownloadUrl => "";
    protected override bool IsExecutable => false;

    public DaveDependency(IServiceProvider services) : base(services) { }
}
