using PamelloV7.Core.Services;

namespace PamelloV7.Server.Services;

public class AssemblyTypeResolver : IAssemblyTypeResolver
{
    public IEnumerable<Type> GetAll() {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
    }

    public IEnumerable<Type> GetInheritors<TType>() {
        return GetAll().Where(x => typeof(TType).IsAssignableFrom(x) && !x.IsAbstract);
    }
}
