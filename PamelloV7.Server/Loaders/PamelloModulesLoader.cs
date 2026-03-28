using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Config.Attributes;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Modules.Loaders;
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
    public readonly Dictionary<string, KeyValuePair<Type, Type>> ConfigTypes;
    
    public PamelloModuleContainer(Assembly assembly, IPamelloModule module) {
        Assembly = assembly;
        Module = module;
        Services = [];
        ConfigTypes = [];
        
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
        
        var rootNodes = assembly.GetTypes().Where(t => t.GetCustomAttribute<ConfigRootAttribute>() is not null);
        foreach (var rootNode in rootNodes) {
            var attribute = rootNode.GetCustomAttribute<ConfigRootAttribute>()!;
            
            var staticType = assembly.GetTypes().FirstOrDefault(t => t.Name == rootNode?.Name.Replace("Node", "Config"));
            var nodeType = assembly.GetTypes().FirstOrDefault(t => t.Name == rootNode?.Name);
        
            if (staticType is null || nodeType is null) return;
        
            ConfigTypes.Add(attribute.Name?.Split(":").LastOrDefault() ?? "", new KeyValuePair<Type, Type>(staticType, nodeType));
        }
    }

    public override string ToString() {
        return $"[{Module.Author}/{Module.Name}] ({Services.Count} services)";
    }
}

public class PamelloModulesLoader : IPamelloModuleLoader
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
            LoadAssembly(typeof(Module.Marsoau.Setup.Setup).Assembly);
        #else
            var loadedModuleAssemblies = new List<Assembly>();
        
            var moduleFiles = Directory.GetFiles(path, "*.pv7m");
            Output.Write($"Loading module files: ({moduleFiles.Length} files)");
        
            foreach (var file in moduleFiles) {
                _moduleContext.PreloadFileToMemory(file);
            }
        
            foreach (var file in moduleFiles) {
                var moduleName = Path.GetFileNameWithoutExtension(file);
        
                try {
                    var assembly = _moduleContext.LoadMainModule(moduleName);
                    loadedModuleAssemblies.Add(assembly);
                    Output.Write($"| Loaded assembly: {moduleName}");
                }
                catch (Exception ex) {
                    Output.Write($"| Failed to load {moduleName}: {ex.Message}", ELogLevel.Warning);
                }
            }

            foreach (var assembly in loadedModuleAssemblies) {
                LoadAssembly(assembly);
            }
        #endif

        Output.Write($"Modules: ({Containers.Count} modules)");
        foreach (var container in Containers) {
            if (container.ConfigTypes.FirstOrDefault().Value is { } type) {
                _configLoader.InitializeFromContainer(container);
                //_configLoader.InitType(type, $"{container.Module.Author}/{container.Module.Name}");
            }
            Output.Write($"{container}\n| {container.Module.Description}");
        }
        
        Output.Write($"Loaded {Containers.Count} modules");
    }
    
    public bool EnsureDependenciesAreSatisfied() {
        Output.Write($"Ensuring modules dependencies are satisfied");;
        
        var notSatisfiedCount = 0;
        
        foreach (var container in Containers) {
            foreach (var dependency in container.Dependencies) {
                var dependencyContainer = Containers.FirstOrDefault(c => c.Assembly.GetName().Name == dependency);
                if (dependencyContainer is null) {
                    Output.Write($"| Module {container.Module.Author}/{container.Module.Name} depends on {dependency}, which is not loaded");
                    notSatisfiedCount++;
                }
            }
        }

        if (notSatisfiedCount > 0) {
            Output.Write($"{notSatisfiedCount} modules dependencies are not satisfied, aborting startup");
        }
        else {
            Output.Write($"All modules dependencies are satisfied");
        }
        
        return notSatisfiedCount == 0;
    }

    public void Configure(IServiceCollection services) {
        Output.Write($"Configuring module services: ({Containers.SelectMany(c => c.Services).Count()} services from {Containers.Count} modules)");
        
        foreach (var container in Containers) {
            Output.Write($"{container}");
            
            foreach (var kvp in container.Services) {
                if (kvp.Value is not null) {
                    Output.Write($"| {kvp.Key.Name} : {kvp.Value.Name}");
                    services.AddSingleton(kvp.Value, kvp.Key);
                }
                else {
                    Output.Write($"| {kvp.Key.Name}");
                    services.AddSingleton(kvp.Key);
                }
            }
            
            container.Module.Configure(services);
        }
        
        Output.Write($"Configuring modules: ({Containers.Count} modules)");
        
        foreach (var container in Containers) {
            Output.Write($"{container}");
            container.Module.Configure(services);
        }
        
        Output.Write($"Modules configured");
    }

    public void Shutdown(IServiceProvider services) {
        Output.Write($"Shutting down module services: ({Containers.SelectMany(c => c.Services).Count()} services from {Containers.Count} modules)");
        
        foreach (var container in Containers) {
            Output.Write($"{container}");
            
            IPamelloService? service;
            
            foreach (var kvp in container.Services) {
                if (kvp.Value is not null) {
                    Output.Write($"| {kvp.Key.Name} : {kvp.Value.Name}");
                    
                    service = services.GetService(kvp.Value) as IPamelloService;
                    if (service is null) continue;
                    
                    service.Shutdown();
                }
                else {
                    Output.Write($"| {kvp.Key.Name}");
                    
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
            Output.Write($"No modules to start in {stage} stage");
            return; 
        }
        
        Output.Write($"Starting modules in {stage} stage: ({stagedContainers.Count} modules)");
        foreach (var container in stagedContainers) {
            Output.Write($"{container}");
            
            _configLoader.FinishFromContainer(container);

            foreach (var (classType, interfaceType) in container.Services) {
                if (classType.GetMethod(nameof(IPamelloService.Startup)) is not null) {
                    var service = (IPamelloService)services.GetRequiredService(interfaceType ?? classType);
                    Output.Write($"| starting {classType.Name}");
                    service.Startup(services);
                }
            }
            
            await container.Module.StartupAsync(services);
        }
        Output.Write($"Started {stagedContainers.Count} modules in {stage} stage");
    }

    public IPamelloModule? GetAssemblyModule(Assembly assembly) {
        return Containers.FirstOrDefault(x => x.Assembly == assembly)?.Module;
    }
}