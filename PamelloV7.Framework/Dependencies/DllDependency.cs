using System.Reflection;
using System.Runtime.InteropServices;

namespace PamelloV7.Framework.Dependencies;

public abstract class DllDependency : SingleFileDependency
{
    protected virtual string[] DllNames => [Name];
    public abstract Assembly DllAssembly { get; }
    
    protected override bool IsExecutable => false;
    
    protected DllDependency(IServiceProvider services) : base(services) { }

    public virtual IntPtr GetResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        if (DllNames.Contains(libraryName) && IsInstalled) return NativeLibrary.Load(GetFile().FullName);
        
        return IntPtr.Zero;
    }
}
