using PamelloV7.Server.Config;
using PamelloV7.Server.Filters;
using PamelloV7.Server.Services;
using System.Reflection;
using System.Text;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.Base;
using PamelloV7.Server.Loaders;

namespace PamelloV7.Server
{
    public class Program
    {
        private IServiceProvider _services;
        private PamelloModulesLoader _modulesLoader;
        
        public static string ConfigPath = "Config/config.json";
        
        private readonly Dictionary<Type, Type?> _assemblyServices;
        
        public WebApplication App { get; set; }
        
        public Program() {
            _assemblyServices = new Dictionary<Type, Type?>();
        }
        
        public static async Task Main(string[] args) => await new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
            StaticLogger.Log($"Starting PamelloV7 {Assembly.GetExecutingAssembly().GetName().Version}");
            
            Console.OutputEncoding = Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);
            
            var modulesLoader = _modulesLoader = new PamelloModulesLoader();
            var configLoader = new PamelloConfigLoader();
            
            LoadAssemblyServices();
            modulesLoader.Load();
            
            if (!modulesLoader.EnsureDependenciesAreSatisfied()) return;
            
            configLoader.Load();
            
            ConfigureAssemblyServices(builder.Services);
            modulesLoader.Configure(builder.Services);
            
            if (!EnsureServicesIsImplemented(builder.Services)) return;
            
            ConfigureApiServices(builder.Services);
            
            builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
            builder.WebHost.UseUrls($"http://{ServerConfig.Root.Host}");
            
            builder.Services.AddSingleton(builder.Services);
            
            App = builder.Build();
            
            _services = App.Services;

            StartupAssemblyServices(App.Services);
            
            foreach (var stage in Enum.GetValues<ELoadingStage>()) {
                await modulesLoader.StartupStage(App.Services, stage);
            }
            
            await StartupApp();
        }

        public void LoadAssemblyServices() {
            var serviceTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof(IPamelloService).IsAssignableFrom(x));
            foreach (var service in serviceTypes) {
                var serviceInterface = service.GetInterfaces()
                    .FirstOrDefault(i => i != typeof(IPamelloService) && typeof(IPamelloService).IsAssignableFrom(i));
                
                _assemblyServices.Add(service, serviceInterface);
            }
        }

        private void ConfigureAssemblyServices(IServiceCollection services) {
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

        private void ShutdownAssemblyServices(IServiceProvider services) {
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

        private void ConfigureApiServices(IServiceCollection services) {
            services.AddControllers(config => config.Filters.Add<PamelloExceptionFilter>());
            services.AddHttpClient();

            services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", builder => {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            
            services.AddHttpContextAccessor();
        }

        private bool EnsureServicesIsImplemented(IServiceCollection services) {
            var requiredServiceTypes = typeof(IPamelloService).Assembly.GetTypes()
                .Where(i => i != typeof(IPamelloService) && typeof(IPamelloService).IsAssignableFrom(i));
            
            var notConfiguredCount = 0;
            var serviceTypes = requiredServiceTypes as Type[] ?? requiredServiceTypes.ToArray();
            
            StaticLogger.Log($"Ensuring required services are implemented: ({serviceTypes.Count()} services)");
            foreach (var requiredServiceType in serviceTypes) {
                var service = services.FirstOrDefault(x => x.ServiceType == requiredServiceType);
                if (service is not null) continue;
                
                notConfiguredCount++;
                Console.WriteLine($"{requiredServiceType.Name} is not implemented");
            }

            if (notConfiguredCount > 0) {
                StaticLogger.Log($"{notConfiguredCount} of required services are not implemented, aborting startup");
            }
            else {
                StaticLogger.Log($"All required services are implemented");
            }
            
            return notConfiguredCount == 0;
        }

        private void StartupAssemblyServices(IServiceProvider services) {
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

        private async Task StartupApp() {
            App.MapControllers();
            App.UseCors("AllowSpecificOrigin");

            var lifetime = App.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(OnStopping);
            lifetime.ApplicationStopped.Register(OnStop);
            lifetime.ApplicationStarted.Register(OnStart);

            await App.RunAsync();
        }

        private void OnStart() {
            var longText = """

            0000000 \                                  00\ 00\       \00\     \000000000 /
            00  __00 \                                 00 |00 |       \00\    \______00 / 
            00 |  00 |000000\  000000\0000\   000000\  00 |00 | 000000\\00\    00 / 00 /  
            0000000  |\____00\ 00  _00  _00\ 00  __00\ 00 |00 |00  __00\\00\  00 / 00 /   
            00  ____/ 0000000 |00 / 00 / 00 |00000000 |00 |00 |00 /  00 |\00\00 / 00 /    
            00 |     00  __00 |00 | 00 | 00 |00   ____|00 |00 |00 |  00 | \000 / 00 /     
            00 |     \0000000 |00 | 00 | 00 |\0000000\ 00 |00 |\000000  |  \0 / 00 /      
            \__|      \_______|\__| \__| \__| \_______|\__|\__| \______/    \/  \_/       

            """;
            var shortText = """
            0000000 \\00\     \000000000 /
            00  __00 \\00\    \______00 / 
            00 |  00 | \00\    00 / 00 /  
            0000000  |  \00\  00 / 00 /   
            00  ____/    \00\00 / 00 /    
            00 |          \000 / 00 /     
            00 |           \0 / 00 /      
            \__|            \/  \_/       
            """;
            var minimalText = "\nPamelloV7 Started Up\n";

            switch (Console.WindowWidth) {
                case >= 80:
                    Console.WriteLine(longText);
                    break;
                case < 30:
                    Console.WriteLine(minimalText);
                    break;
                default:
                    Console.WriteLine(shortText);
                    break;
            }
            
            var events = App.Services.GetRequiredService<IEventsService>();
            events.Invoke(new PamelloStarted() {
                Services = App.Services
            });
        }

        private void OnStopping() {
            Console.WriteLine("\n---\nSTOPPING\n---");
            
            _modulesLoader.Shutdown(_services);
            ShutdownAssemblyServices(_services);
        }
        private void OnStop() {
            Console.WriteLine("\n---\nSTOPPED\n---");
        }
    }
}

//C17
