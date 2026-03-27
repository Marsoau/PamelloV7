using System.Reflection;

namespace PamelloV7.Framework.Modules.Loaders;

public interface IPamelloModuleLoader
{
    public IPamelloModule? GetAssemblyModule(Assembly assembly);
}
