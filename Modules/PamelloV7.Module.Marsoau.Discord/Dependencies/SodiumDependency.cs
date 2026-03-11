using PamelloV7.Framework.Dependencies;

namespace PamelloV7.Module.Marsoau.Discord.Dependencies;

public class SodiumDependency : SimpleDependency
{
    public override string Name => "sodium";
    public override string InternalFilePath => "libsodium.so";

    protected override string VersionProperty => "";
    protected override string VersionUrl => "";
    protected override string DownloadUrl => "";
    protected override bool IsExecutable => false;

    public SodiumDependency(IServiceProvider services) : base(services) { }
}
