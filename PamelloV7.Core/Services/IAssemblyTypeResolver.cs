using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IAssemblyTypeResolver : IPamelloService
{
    public IEnumerable<Type> GetAll();
    public Type? GetByName(string name);
    public IEnumerable<Type> GetWithAttribute<TAttribute>();
    public IEnumerable<Type> GetInheritorsOf<TType>();
    public IEnumerable<Type> GetInheritorsOf(params Type[] types);
    public Type? GetTypeByName(string name);
}
