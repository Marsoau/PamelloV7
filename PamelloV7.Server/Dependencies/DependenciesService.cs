using System.Reflection;
using System.Runtime.InteropServices;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Services;

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

        var dllResolvers = new Dictionary<Assembly, List<LibDependency>>();

        Output.Write($"Loading Dependencies: ({types.Count})");
        foreach (var type in types) {
            Output.Write($"| {type}");
            
            var dependency = (Dependency)Activator.CreateInstance(type, services)!;
            dependency.Startup();

            if (dependency is LibDependency dllDependency) {
                if (dllResolvers.TryGetValue(dllDependency.DllAssembly, out var resolvers))
                    resolvers.Add(dllDependency);
                else dllResolvers.Add(dllDependency.DllAssembly, [dllDependency]);
            }
            
            Dependencies.Add(dependency);
        }

        Output.Write($"Mapping Dll Resolvers: ({dllResolvers.Count})");
        foreach (var (assembly, dependencies) in dllResolvers) {
            Output.Write($"| {assembly.FullName} : {string.Join(", ", dependencies.Select(d => d.Name))}");
            NativeLibrary.SetDllImportResolver(assembly, ((name, b, c) => {
                foreach (var result in dependencies.Select(dependency => dependency.GetResolver(name, b, c)).Where(result => result != IntPtr.Zero)) {
                    return result;
                }

                Output.Write($"Dependency \"{name}\" not found in overriden resolver for {assembly.FullName}");

                return IntPtr.Zero;
            }));
        }
    }

    public Dependency? Resolve(string name) {
        return Dependencies.FirstOrDefault(d => d.Name == name);
    }
}
