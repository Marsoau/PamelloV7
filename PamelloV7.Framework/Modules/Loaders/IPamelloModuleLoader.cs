using System.Reflection;
using PamelloV7.Framework.Modules.Containers;

namespace PamelloV7.Framework.Modules.Loaders;

public interface IPamelloModuleLoader
{
    public List<PamelloModuleContainer> Containers { get; }
    public IPamelloModule? GetAssemblyModule(Assembly assembly);
}
