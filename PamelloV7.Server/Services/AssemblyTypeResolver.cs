using System.Reflection;
using PamelloV7.Framework.Config.Attributes;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Loaders;

namespace PamelloV7.Server.Services;

public class AssemblyTypeResolver : IAssemblyTypeResolver
{
    private PamelloModulesLoader ModulesLoader { get; set; } = null!;
    
    public IEnumerable<Type> GetAll() {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
    }

    public void LoadModules(PamelloModulesLoader loader, IServiceProvider services) {
        ModulesLoader = loader;
    }
    
    public Type? GetByName(string name) {
        return GetAll().FirstOrDefault(x => x.Name == name);
    }
    public Type? GetByFullName(string fullName) {
        return GetAll().FirstOrDefault(x => x.FullName == fullName);
    }

    public IEnumerable<Type> GetWithAttribute<TAttribute>() {
        return GetAll().Where(x => x.GetCustomAttribute(typeof(TAttribute)) is not null);
    }

    public IEnumerable<Type> GetInheritorsOf<TType>() {
        return GetAll().Where(x => x.IsAssignableTo(typeof(TType)) && !x.IsAbstract);
    }
    public IEnumerable<Type> GetInheritorsOf(params Type[] types) {
        return GetAll().Where(x => types.Any(t => t.IsAssignableFrom(x)) && !x.IsAbstract);
    }
    
    public Type? GetTypeByName(string name) {
        return GetAll().FirstOrDefault(x => x.Name == name);
    }

    public IPamelloModule? GetAssemblyModule(Assembly assembly) {
        return ModulesLoader.Containers.FirstOrDefault(x => x.Assembly == assembly)?.Module;
    }
}
