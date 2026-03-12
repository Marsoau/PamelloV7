using System.Reflection;

namespace PamelloV7.Framework.Dependencies;

public abstract class MarsoauLibDependency : LibDependency
{
    protected override string VersionProperty => "";
    protected override string VersionUrl => "";

    private static string DownloadUrl => "https://storage.marsoau.com/share/libs";
    protected override string DownloadUrlLinux => $"{DownloadUrl}/{Name}/linux";
    protected override string DownloadUrlWindows => $"{DownloadUrl}/{Name}/windows";
    public override bool IsExecutable => false;

    protected MarsoauLibDependency(IServiceProvider services) : base(services) { }
}
