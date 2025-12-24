using System.Reflection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Services.Base;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Loaders;

public class PamelloModuleContainer
{
    public readonly Assembly Assembly;
    public readonly IPamelloModule Module;
    public readonly Dictionary<Type, Type?> Services;
    public readonly List<string> Dependencies;
    
    public PamelloModuleContainer(Assembly assembly, IPamelloModule module) {
        Assembly = assembly;
        Module = module;
        Services = [];
        
        var serviceTypes = assembly.GetTypes().Where(x => typeof(IPamelloService).IsAssignableFrom(x));
        foreach (var service in serviceTypes) {
            var serviceInterface = service.GetInterfaces()
                .FirstOrDefault(i => i != typeof(IPamelloService) && typeof(IPamelloService).IsAssignableFrom(i));
            
            Services.Add(service, serviceInterface);
        }
            
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        Dependencies = referencedAssemblies
            .Select(x => x.Name!)
            .Where(name => name.StartsWith("PamelloV7.Module"))
            .ToList();
    }

    public override string ToString() {
        return $"[{Module.Author}/{Module.Name}] ({Services.Count} services)";
    }
}

public class PamelloModulesLoader
{
    public readonly List<PamelloModuleContainer> Containers;
    
    public PamelloModulesLoader() {
        Containers = [];
    }

    public void Load() {
        var path = Path.Combine(AppContext.BaseDirectory, "Modules");
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
            return;
        }
        
        var moduleFiles = Directory.GetFiles(path, "*.dll");
        StaticLogger.Log($"Loading modules: ({moduleFiles.Length} files)");
        
        foreach (var moduleFile in moduleFiles) {
            var assembly = Assembly.LoadFrom(moduleFile);
            
            var moduleType = assembly.GetTypes().FirstOrDefault(x => typeof(IPamelloModule).IsAssignableFrom(x));
            if (moduleType is null) continue;
            
            if (Activator.CreateInstance(moduleType) is not IPamelloModule module) continue;
            
            var container = new PamelloModuleContainer(assembly, module);

            Console.WriteLine($"{container}\n| {module.Description}");
            
            Containers.Add(container);
        }
        
        StaticLogger.Log($"Loaded {Containers.Count} modules");
    }

    public bool EnsureDependenciesAreSatisfied() {
        StaticLogger.Log($"Ensuring modules dependencies are satisfied");;
        
        var notSatisfiedCount = 0;
        
        foreach (var container in Containers) {
            foreach (var dependency in container.Dependencies) {
                var dependencyContainer = Containers.FirstOrDefault(c => c.Assembly.GetName().Name == dependency);
                if (dependencyContainer is null) {
                    Console.WriteLine($"| Module {container.Module.Author}/{container.Module.Name} depends on {dependency}, which is not loaded");
                    notSatisfiedCount++;
                }
            }
        }

        if (notSatisfiedCount > 0) {
            StaticLogger.Log($"{notSatisfiedCount} modules dependencies are not satisfied, aborting startup");
        }
        else {
            StaticLogger.Log($"All modules dependencies are satisfied");
        }
        
        return notSatisfiedCount == 0;
    }

    public void Configure(IServiceCollection services) {
        StaticLogger.Log($"Configuring module services: ({Containers.SelectMany(c => c.Services).Count()} services from {Containers.Count} modules)");
        
        foreach (var container in Containers) {
            Console.WriteLine($"{container}");
            
            foreach (var kvp in container.Services) {
                if (kvp.Value is not null) {
                    Console.WriteLine($"| {kvp.Key.Name} : {kvp.Value.Name}");
                    services.AddSingleton(kvp.Value, kvp.Key);
                }
                else {
                    Console.WriteLine($"| {kvp.Key.Name}");
                    services.AddSingleton(kvp.Key);
                }
            }
            
            container.Module.Configure(services);
        }
        
        StaticLogger.Log($"Configuring modules: ({Containers.Count} modules)");
        
        foreach (var container in Containers) {
            Console.WriteLine($"{container}");
            container.Module.Configure(services);
        }
        
        StaticLogger.Log($"Modules configured");
    }

    public void StartupStage(IServiceProvider services, ELoadingStage stage) {
        var stagedContainers = Containers.Where(container => container.Module.Stage == stage).ToList();

        if (stagedContainers.Count == 0) {
            StaticLogger.Log($"No modules to start in {stage} stage");
            return; 
        }
        
        StaticLogger.Log($"Starting modules in {stage} stage: ({stagedContainers.Count} modules)");
        foreach (var container in stagedContainers) {
            Console.WriteLine($"{container}");
            container.Module.Startup(services);
        }
        StaticLogger.Log($"Started {stagedContainers.Count} modules in {stage} stage");
    }
}