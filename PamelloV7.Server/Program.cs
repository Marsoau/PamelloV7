using System.ComponentModel;
using PamelloV7.Server.Config;
using PamelloV7.Server.Filters;
using PamelloV7.Server.Services;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.Base;
using PamelloV7.Server.Hubs;
using PamelloV7.Server.Loaders;

namespace PamelloV7.Server
{
    public class Program
    {
        private IServiceProvider _services;
        
        public static string ConfigPath = "Config/config.jsonc";
        
        private PamelloModulesLoader _modulesLoader;
        private PamelloServerLoader _serverLoader;
        
        public WebApplication App { get; set; }
        
        public static async Task Main(string[] args) => await new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
            StaticLogger.Log($"Starting PamelloV7 {Assembly.GetExecutingAssembly().GetName().Version}");
            
            //remove later
            var _ = typeof(Audio.Services.PamelloAudioSystem);
            
            Console.OutputEncoding = Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);
            
            var configLoader = new PamelloConfigLoader();
            _serverLoader = new PamelloServerLoader(configLoader);
            _modulesLoader = new PamelloModulesLoader(configLoader);
            
            configLoader.Load();
            _serverLoader.Load();
            _modulesLoader.Load();
            
            if (!_modulesLoader.EnsureDependenciesAreSatisfied()) return;
            
            _serverLoader.ConfigureAssemblyServices(builder.Services);
            _modulesLoader.Configure(builder.Services);
            
            if (!EnsureServicesIsImplemented(builder.Services)) return;
            
            _serverLoader.ConfigureApiServices(builder.Services);
            
            builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
            builder.WebHost.UseUrls($"{ServerConfig.Root.Host}");
            
            builder.Services.AddSingleton(builder.Services);
            
            App = builder.Build();
            
            _services = App.Services;

            _serverLoader.StartupAssemblyServices(App.Services);
            
            foreach (var stage in Enum.GetValues<ELoadingStage>()) {
                await _modulesLoader.StartupStage(App.Services, stage);
            }
            
            await StartupApp();
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

        private async Task StartupApp() {
            App.MapHub<SignalHub>("/Signal");
            App.MapControllers();
            App.UseCors();

            var lifetime = App.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(OnStopping);
            lifetime.ApplicationStopped.Register(OnStop);
            lifetime.ApplicationStarted.Register(OnStart);

            await App.RunAsync();
        }

        private void OnStart() {
            Console.WriteLine(Console.WindowWidth switch {
                >= 80 => """
                         
                         0000000 \                                  00\ 00\       \00\     \000000000 /
                         00  __00 \                                 00 |00 |       \00\    \______00 / 
                         00 |  00 |000000\  000000\0000\   000000\  00 |00 | 000000\\00\    00 / 00 /  
                         0000000  |\____00\ 00  _00  _00\ 00  __00\ 00 |00 |00  __00\\00\  00 / 00 /   
                         00  ____/ 0000000 |00 / 00 / 00 |00000000 |00 |00 |00 /  00 |\00\00 / 00 /    
                         00 |     00  __00 |00 | 00 | 00 |00   ____|00 |00 |00 |  00 | \000 / 00 /     
                         00 |     \0000000 |00 | 00 | 00 |\0000000\ 00 |00 |\000000  |  \0 / 00 /      
                         \__|      \_______|\__| \__| \__| \_______|\__|\__| \______/    \/  \_/       
                         
                         """,
                < 30 => """
                        
                        0000000 \\00\     \000000000 /
                        00  __00 \\00\    \______00 / 
                        00 |  00 | \00\    00 / 00 /  
                        0000000  |  \00\  00 / 00 /   
                        00  ____/    \00\00 / 00 /    
                        00 |          \000 / 00 /     
                        00 |           \0 / 00 /      
                        \__|            \/  \_/       
                        
                        """,
                _ => "\nPamelloV7 Started Up\n"
            });
            
            var events = App.Services.GetRequiredService<IEventsService>();
            events.Invoke(new PamelloStarted() {
                Services = App.Services
            });
        }

        private void OnStopping() {
            Console.WriteLine("\n---\nSTOPPING\n---");
            
            _modulesLoader.Shutdown(_services);
            _serverLoader.ShutdownAssemblyServices(_services);
        }
        private void OnStop() {
            Console.WriteLine("\n---\nSTOPPED\n---");
        }
    }
}

//C17
