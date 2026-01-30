using System.Reflection;
using System.Text.Json.Serialization;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Services.Base;
using PamelloV7.Server.Config;
using PamelloV7.Server.Filters;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Loaders;

public class PamelloServerLoader
{
    private readonly PamelloConfigLoader _configLoader;
    
    private readonly Dictionary<Type, Type?> _assemblyServices;
    
    public PamelloServerLoader(PamelloConfigLoader configLoader) {
        _configLoader = configLoader;
        
        _assemblyServices = new Dictionary<Type, Type?>();
    }
    
    public void Load() {
        var serviceTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof(IPamelloService).IsAssignableFrom(x));
        foreach (var service in serviceTypes) {
            var serviceInterface = service.GetInterfaces()
                .FirstOrDefault(i => i != typeof(IPamelloService) && typeof(IPamelloService).IsAssignableFrom(i));
                
            _assemblyServices.Add(service, serviceInterface);
        }
        
        _configLoader.InitType(typeof(ServerConfig), "Server");
    }
    
    public void ConfigureAssemblyServices(IServiceCollection services) {
        StaticLogger.Log($"Configuring server services: ({_assemblyServices.Count} services)");
        foreach (var kvp in _assemblyServices) {
            if (kvp.Value is not null) {
                Console.WriteLine($"| {kvp.Key.Name} : {kvp.Value.Name}");
                services.AddSingleton(kvp.Value, kvp.Key);
            }
            else {
                Console.WriteLine($"| {kvp.Key.Name}");
                services.AddSingleton(kvp.Key);
            }
        }
        StaticLogger.Log($"Server services configured");
    }
    
    public void ConfigureApiServices(IServiceCollection services) {
        services.AddControllers(config => config.Filters.Add<PamelloExceptionFilter>())
            .AddJsonOptions(options =>
            {
                var entitiesOptions = JsonEntitiesFactory.Options;

                options.JsonSerializerOptions.ReferenceHandler = entitiesOptions.ReferenceHandler;
                options.JsonSerializerOptions.PropertyNamingPolicy = entitiesOptions.PropertyNamingPolicy;
                options.JsonSerializerOptions.DefaultIgnoreCondition = entitiesOptions.DefaultIgnoreCondition;

                foreach (var converter in entitiesOptions.Converters)
                {
                    options.JsonSerializerOptions.Converters.Add(converter);
                }
            });
        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                var entitiesOptions = JsonEntitiesFactory.Options;

                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.PayloadSerializerOptions.PropertyNamingPolicy = entitiesOptions.PropertyNamingPolicy;
                options.PayloadSerializerOptions.DefaultIgnoreCondition = entitiesOptions.DefaultIgnoreCondition;

                foreach (var converter in entitiesOptions.Converters)
                {
                    options.PayloadSerializerOptions.Converters.Add(converter);
                }
            });
        services.AddHttpClient();

        services.AddCors(options => {
            options.AddDefaultPolicy(builder => builder
                .SetIsOriginAllowed(origin => true)
                //.WithOrigins("http://127.0.0.1:41630", "null") // "null" allows opening HTML directly from file system
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
        });
            
        services.AddHttpContextAccessor();
    }
    
    public void StartupAssemblyServices(IServiceProvider services) {
        object? result;
        
        StaticLogger.Log($"Starting server services: ({_assemblyServices.Count} services)");
        foreach (var kvp in _assemblyServices) {
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
        StaticLogger.Log($"Server services started");
    }
    
    public void ShutdownAssemblyServices(IServiceProvider services) {
        StaticLogger.Log($"Stopping server services: ({_assemblyServices.Count} services)");
            
        IPamelloService? service;
            
        foreach (var kvp in _assemblyServices) {
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
        StaticLogger.Log($"Server services stopped");
    }
}
