using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Config.Attributes;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Server.Loaders.Context;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Loaders;

public class PamelloModuleContainer
{
    public readonly Assembly Assembly;
    public readonly IPamelloModule Module;
    public readonly Dictionary<Type, Type?> Services;
    public readonly List<string> Dependencies;
    public readonly Type? ConfigType;
    
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
        
        var rootNode = assembly.GetTypes().FirstOrDefault(t => t.GetCustomAttribute<ConfigRootAttribute>() is not null);
        ConfigType = assembly.GetTypes().FirstOrDefault(t => t.Name == $"{rootNode?.Name.Replace("Node", "")}Config");
    }

    public override string ToString() {
        return $"[{Module.Author}/{Module.Name}] ({Services.Count} services)";
    }
}

public class PamelloModulesLoader
{
    private readonly PamelloConfigLoader _configLoader;

    public readonly List<PamelloModuleContainer> Containers;

    private readonly PamelloModuleLoadContext _moduleContext;
    
    public PamelloModulesLoader(PamelloConfigLoader configLoader) {
        _configLoader = configLoader;
        
        _moduleContext = new PamelloModuleLoadContext();
        
        Containers = [];
    }
    
    private void LoadAssembly(Assembly assembly) {
        var moduleType = assembly.GetTypes().FirstOrDefault(x => typeof(IPamelloModule).IsAssignableFrom(x));
                
        if (moduleType is null) return;
        if (Activator.CreateInstance(moduleType) is not IPamelloModule module) return;
                
        Containers.Add(new PamelloModuleContainer(assembly, module));
    }

    public void Load() {
        var path = Path.Combine(AppContext.BaseDirectory, "Modules");
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
            return;
        }
        
        #if DEBUG
            LoadAssembly(typeof(Module.Marsoau.Base.Base).Assembly);
            LoadAssembly(typeof(Module.Marsoau.Database.Database).Assembly);
            LoadAssembly(typeof(Module.Marsoau.Discord.Discord).Assembly);
            LoadAssembly(typeof(Module.Marsoau.Osu.Osu).Assembly);
            LoadAssembly(typeof(Module.Marsoau.PEQL.PEQL).Assembly);
            LoadAssembly(typeof(Module.Marsoau.Test.Test).Assembly);
            LoadAssembly(typeof(Module.Marsoau.YouTube.YouTube).Assembly);
        #else
            var loadedModuleAssemblies = new List<Assembly>();
        
            var moduleFiles = Directory.GetFiles(path, "*.pv7m");
            StaticLogger.Log($"Loading modules: ({moduleFiles.Length} files)");
        
            foreach (var file in moduleFiles) {
                _moduleContext.PreloadFileToMemory(file);
            }
        
            foreach (var file in moduleFiles) {
                var moduleName = Path.GetFileNameWithoutExtension(file);
        
                try {
                    var assembly = _moduleContext.LoadMainModule(moduleName);
                    loadedModuleAssemblies.Add(assembly);
                    Console.WriteLine($"[Loader] Loaded assembly: {moduleName}");
                }
                catch (Exception ex) {
                    Console.WriteLine($"[Loader] Failed to load {moduleName}: {ex.Message}");
                }
            }

            foreach (var assembly in loadedModuleAssemblies) {
                LoadAssembly(assembly);
            }
        #endif

        foreach (var container in Containers) {
            if (container.ConfigType is not null) {
                _configLoader.InitType(container.ConfigType, $"{container.Module.Author}/{container.Module.Name}");
            }
            Console.WriteLine($"{container}\n| {container.Module.Description}");
        }
        
        StaticLogger.Log($"Loaded {Containers.Count} modules");
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private bool IsModuleDisabled(string dllPath) {
        var context = new AssemblyLoadContext("ModuleCheck", isCollectible: true);

        try {
            var assembly = context.LoadFromAssemblyPath(dllPath);

            var moduleType = assembly.GetTypes().FirstOrDefault(x => typeof(IPamelloModule).IsAssignableFrom(x));
            if (moduleType is null) return true;

            if (Activator.CreateInstance(moduleType) is not IPamelloModule module) return true;

            var isDisabled = ServerConfig.Root.DisabledModules.Contains($"{module.Author}/{module.Name}");

            if (isDisabled) Console.WriteLine($"[{module.Author}/{module.Name}] DISABLED\n| {module.Description}");
            
            return isDisabled;
        }
        finally {
            context.Unload();
        }
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

    public void Shutdown(IServiceProvider services) {
        StaticLogger.Log($"Shutting down module services: ({Containers.SelectMany(c => c.Services).Count()} services from {Containers.Count} modules)");
        
        foreach (var container in Containers) {
            Console.WriteLine($"{container}");
            
            IPamelloService? service;
            
            foreach (var kvp in container.Services) {
                if (kvp.Value is not null) {
                    Console.WriteLine($"| {kvp.Key.Name} : {kvp.Value.Name}");
                    
                    service = services.GetService(kvp.Value) as IPamelloService;
                    if (service is null) continue;
                    
                    service.Shutdown();
                }
                else {
                    Console.WriteLine($"| {kvp.Key.Name}");
                    
                    service = services.GetService(kvp.Key) as IPamelloService;
                    if (service is null) continue;
                    
                    service.Shutdown();
                }
            }
        }
    }

    public async Task StartupStage(IServiceProvider services, ELoadingStage stage) {
        var stagedContainers = Containers.Where(container => container.Module.Stage == stage).ToList();

        if (stagedContainers.Count == 0) {
            StaticLogger.Log($"No modules to start in {stage} stage");
            return; 
        }
        
        StaticLogger.Log($"Starting modules in {stage} stage: ({stagedContainers.Count} modules)");
        foreach (var container in stagedContainers) {
            Console.WriteLine($"{container}");
            await container.Module.StartupAsync(services);

            foreach (var (classType, interfaceType) in container.Services) {
                if (classType.GetMethod(nameof(IPamelloService.Startup)) is not null) {
                    var service = (IPamelloService)services.GetRequiredService(interfaceType ?? classType);
                    Console.WriteLine($"| starting {classType.Name}");
                    service.Startup(services);
                }
            }
        }
        StaticLogger.Log($"Started {stagedContainers.Count} modules in {stage} stage");
    }
}