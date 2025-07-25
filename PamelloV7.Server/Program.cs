using PamelloV7.Server.Config;
using PamelloV7.Server.Filters;
using PamelloV7.Server.Services;
using System.Reflection;
using System.Text;
using PamelloV7.Core.Services.Base;
using PamelloV7.Server.Loaders;
using PamelloV7.Server.Plugins;

namespace PamelloV7.Server
{
    public class Program
    {
        private readonly Dictionary<Type, Type?> _assemblyServices;
        
        public Program() {
            _assemblyServices = new Dictionary<Type, Type?>();
        }
        
        public static async Task Main(string[] args) => await new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
            StaticLogger.Log($"Starting PamelloV7 {Assembly.GetExecutingAssembly().GetName().Version}");
            
            Console.OutputEncoding = Encoding.Unicode;

            var builder = WebApplication.CreateBuilder(args);
            var pluginLoader = new PamelloPluginLoader();
            
            LoadAssemblyServices();
            pluginLoader.Load();
            
            ConfigureAssemblyServices(builder.Services);
            pluginLoader.Configure(builder.Services);
            
            if (!EnsureServicesIsImplemented(builder.Services)) return;
            
            ConfigureApiServices(builder.Services);
            
            DatabaseRepositoriesLoader.Configure(builder.Services);
            
            builder.WebHost.UseUrls($"http://{PamelloServerConfig.Root.Host}");
            
            var app = builder.Build();
            
            await DatabaseRepositoriesLoader.Load(app.Services);

            StartupAssemblyServices(app.Services);
            pluginLoader.Startup(app.Services);
            
            await StartupApiServices(app);
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
                    Console.WriteLine($"{kvp.Key.Name} : {kvp.Value.Name}");
                    services.AddSingleton(kvp.Value, kvp.Key);
                }
                else {
                    Console.WriteLine($"{kvp.Key.Name}");
                    services.AddSingleton(kvp.Key);
                }
            }
            StaticLogger.Log($"Server services configured");
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
            
            StaticLogger.Log($"Ensuring required services are implemented: ({requiredServiceTypes.Count()} services)");
            foreach (var requiredServiceType in requiredServiceTypes) {
                var service = services.FirstOrDefault(x => x.ServiceType == requiredServiceType);
                if (service is not null) continue;
                
                notConfiguredCount++;
                Console.WriteLine($"{requiredServiceType.Name} is not implemented");
            }

            if (notConfiguredCount > 0) {
                StaticLogger.Log($"{notConfiguredCount} of required services are not implemented, aborting startup");
            }
            else {
                StaticLogger.Log($"Required services are implemented");
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

        private async Task StartupApiServices(WebApplication app) {
            app.MapControllers();
            app.UseCors("AllowSpecificOrigin");

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(OnStopping);
            lifetime.ApplicationStopped.Register(OnStop);
            lifetime.ApplicationStarted.Register(OnStart);

            await app.RunAsync();
        }

        private void OnStart() {
            Console.WriteLine("START");
        }

        private void OnStop() {
            Console.WriteLine("STOP");
        }
        private void OnStopping() {
            Console.WriteLine("STOPPING");
        }
    }
}

//C17
