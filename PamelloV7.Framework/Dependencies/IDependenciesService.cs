using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Dependencies;

public interface IDependenciesService : IPamelloService
{
    public Dependency Resolve(string name);
}
