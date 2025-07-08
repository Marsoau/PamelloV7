using System.Reflection;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Services.Base;

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
        Console.WriteLine($"Found {pluginFiles.Length} plugin files");
        
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

            Console.WriteLine($"Loaded plugin: [{plugin.Name}] {plugin.Description}");
            
            Plugins.Add(plugin);
        }
    }

    public void Configure(IServiceCollection services) {
        foreach (var kvp in Services) {
            
            if (kvp.Value is not null) {
                Console.WriteLine($"Configuring service: {kvp.Key.Name} : {kvp.Value.Name}");
                services.AddSingleton(kvp.Value, kvp.Key);
            }
            else {
                Console.WriteLine($"Configuring service: {kvp.Key.Name}");
                services.AddSingleton(kvp.Key);
            }
        }
        
        foreach (var plugin in Plugins) {
            plugin.Configure(services);
        }
    }

    public void Startup(IServiceProvider services) {
        object? result;
        
        foreach (var kvp in Services) {
            if (kvp.Value is not null) {
                Console.WriteLine($"Starting service: {kvp.Key.Name} : {kvp.Value.Name}");
                result = services.GetService(kvp.Value);
            }
            else {
                Console.WriteLine($"Starting service: {kvp.Key.Name}");
                result = services.GetService(kvp.Key);
            }
            
            if (result is not null) {
                ((IPamelloService)result).Startup(services);
            }
            else {
                Console.WriteLine($"Service {kvp.Key.Name} failed to start");
            }
        }
        
        
        
        foreach (var plugin in Plugins) {
            plugin.Startup(services);
        }
    }
    
    public void Shutdown() {}
}
