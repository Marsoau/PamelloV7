using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Dependencies.Service;

public interface IDependenciesService : IPamelloService
{
    public IEnumerable<Dependency> GetAll();

    public Dependency ResolveRequired(string name, bool installedRequired = true) {
        var dependency = Resolve(name);
        if (dependency is null) throw new PamelloException($"Required dependency \"{name}\" not found");
        
        if (installedRequired && !dependency.IsInstalled) throw new PamelloException($"Required dependency \"{name}\" is not installed");
        
        return dependency;
    }
    public Dependency? Resolve(string name);
}
