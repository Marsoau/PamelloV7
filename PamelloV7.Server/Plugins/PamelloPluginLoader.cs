using System.Reflection;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.Base;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Plugins;

public class PamelloPluginLoader
{
    public readonly List<IPamelloPlugin> Plugins;
    public readonly Dictionary<Type, Type?> Services;
    
    public PamelloPluginLoader() {
        Plugins = [];
        Services = [];
    }

    public void Load() {
        var path = Path.Combine(AppContext.BaseDirectory, "Plugins");
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
            return;
        }
        
        var pluginFiles = Directory.GetFiles(path, "*.dll");
        StaticLogger.Log($"Loading plugins: ({pluginFiles.Length} files)");
        var count = 0;
        
        foreach (var pluginFile in pluginFiles) {
            var assembly = Assembly.LoadFrom(pluginFile);
            
            var serviceTypes = assembly.GetTypes().Where(x => typeof(IPamelloService).IsAssignableFrom(x));
            foreach (var service in serviceTypes) {
                var serviceInterface = service.GetInterfaces()
                    .FirstOrDefault(i => i != typeof(IPamelloService) && typeof(IPamelloService).IsAssignableFrom(i));
                
                Services.Add(service, serviceInterface);
            }
            
            var pluginType = assembly.GetTypes().FirstOrDefault(x => typeof(IPamelloPlugin).IsAssignableFrom(x));
            if (pluginType is null) continue;
            
            if (Activator.CreateInstance(pluginType) is not IPamelloPlugin plugin) continue;

            Console.WriteLine($"[{plugin.Name}] {plugin.Description}");
            
            count++;
            Plugins.Add(plugin);
        }
        
        StaticLogger.Log($"Loaded {count} plugins");
    }

    public void Configure(IServiceCollection services) {
        StaticLogger.Log($"Configuring services: ({Services.Count} services)");
        foreach (var kvp in Services) {
            if (kvp.Value is not null) {
                Console.WriteLine($"{kvp.Key.Name} : {kvp.Value.Name}");
                services.AddSingleton(kvp.Value, kvp.Key);
            }
            else {
                Console.WriteLine($"{kvp.Key.Name}");
                services.AddSingleton(kvp.Key);
            }
        }
        StaticLogger.Log($"Services configured");
        
        StaticLogger.Log($"Configuring plugins: ({Plugins.Count} plugins)");
        foreach (var plugin in Plugins) {
            Console.WriteLine($"[{plugin.Name}]");
            plugin.Configure(services);
        }
        StaticLogger.Log($"Plugins configured");;
    }

    public void Startup(IServiceProvider services) {
        object? result;
        
        StaticLogger.Log($"Starting services: ({Services.Count} services)");
        foreach (var kvp in Services) {
            if (kvp.Value is not null) {
                Console.WriteLine($"{kvp.Key.Name} : {kvp.Value.Name}");
                result = services.GetService(kvp.Value);
            }
            else {
                Console.WriteLine($"{kvp.Key.Name}");
                result = services.GetService(kvp.Key);
            }
            
            if (result is not null) {
                ((IPamelloService)result).Startup(services);
            }
            else {
                Console.WriteLine($"Service {kvp.Key.Name} failed to start");
            }
        }
        StaticLogger.Log($"Services started");
        
        StaticLogger.Log($"Starting plugins: ({Plugins.Count} plugins)");
        foreach (var plugin in Plugins) {
            Console.WriteLine($"[{plugin.Name}]");
            plugin.Startup(services);
        }
        StaticLogger.Log($"Plugins started");
    }
    
    public void Shutdown() {}
}
