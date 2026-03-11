using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class OpusDependency : SimpleDependency
{
    public override string Name => "opus";
    public override string InternalFilePath => "libopus.so";

    protected override string VersionProperty => "";
    protected override string VersionUrl => "";
    protected override string DownloadUrl => "";
    protected override bool IsExecutable => false;

    public OpusDependency(IServiceProvider services) : base(services) { }
}
