using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Dependencies;

public interface IDependenciesService : IPamelloService
{
    public IEnumerable<Dependency> GetAll();
    
    public Dependency ResolveRequired(string name)
        => Resolve(name) ?? throw new PamelloException($"Required dependency \"{name}\" not found");
    public Dependency? Resolve(string name);
}
