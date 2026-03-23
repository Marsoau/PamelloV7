using System.Reflection;
using System.Runtime.InteropServices;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Framework.Dependencies;

public abstract class LibDependency : SingleFileDependency
{
    protected virtual string[] DllNames => [Name];
    public abstract Assembly DllAssembly { get; }
    
    public override bool IsExecutable => false;
    
    protected LibDependency(IServiceProvider services) : base(services) { }

    public virtual IntPtr GetResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        if (!DllNames.Contains(libraryName)) return IntPtr.Zero;
        
        if (IsInstalled) return NativeLibrary.Load(GetFile().FullName);
            
        Output.Write($"Custom dependency is requested but not installed! name: \"{Name}\" path: {GetFile().FullName}");
        
        return IntPtr.Zero;
    }
}
