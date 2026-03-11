using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Services;

namespace PamelloV7.Server.Dependencies;

public class DependenciesService : IDependenciesService
{
    public List<Dependency> Dependencies { get; } = [];

    public IEnumerable<Dependency> GetAll() {
        return Dependencies;
    }

    public void Startup(IServiceProvider services) {
        var typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
        var types = typeResolver.GetInheritorsOf<Dependency>().ToList();

        Console.WriteLine($"Dependencies: {types.Count}");
        foreach (var type in types) {
            Console.WriteLine($"| {type}");
            Dependencies.Add((Dependency)Activator.CreateInstance(type, services)!);
        }
    }

    public Dependency? Resolve(string name) {
        return Dependencies.FirstOrDefault(d => d.Name == name);
    }
}
