using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Dependencies;

public interface IDependenciesService : IPamelloService
{
    public Dependency ResolveRequired(string name)
        => Resolve(name) ?? throw new PamelloException($"Dependency \"{name}\" not found");
    public Dependency? Resolve(string name);
}
