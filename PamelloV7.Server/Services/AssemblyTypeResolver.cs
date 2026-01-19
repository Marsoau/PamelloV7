using System.Reflection;
using PamelloV7.Core.Services;

namespace PamelloV7.Server.Services;

public class AssemblyTypeResolver : IAssemblyTypeResolver
{
    public IEnumerable<Type> GetAll() {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
    }
    
    public IEnumerable<Type> GetWithAttribute<TAttribute>() {
        return GetAll().Where(x => x.GetCustomAttribute(typeof(TAttribute)) != null);
    }

    public IEnumerable<Type> GetInheritorsOf<TType>() {
        return GetAll().Where(x => typeof(TType).IsAssignableFrom(x) && !x.IsAbstract);
    }
    
    public Type? GetTypeByName(string name) {
        return GetAll().FirstOrDefault(x => x.Name == name);
    }
}
