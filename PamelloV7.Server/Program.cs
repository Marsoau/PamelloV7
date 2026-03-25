using System.ComponentModel;
using PamelloV7.Server.Filters;
using PamelloV7.Server.Services;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Avalonia;
using Consolonia;
using Microsoft.AspNetCore.SignalR;
using PamelloV7.Audio.Modules;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Converters;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Logging.Services;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Server.Consolonia;
using PamelloV7.Server.Consolonia.Screens;
using PamelloV7.Server.Consolonia.Windows;
using PamelloV7.Server.Hubs;
using PamelloV7.Server.Loaders;
using PamelloV7.Server.Logging;
using IApplicationLifetime = Avalonia.Controls.ApplicationLifetimes.IApplicationLifetime;

namespace PamelloV7.Server
{
    public class Program
    {
        private IServiceProvider _services;
        
        public static string ConfigPath = "Config/config.jsonc";
        
        private PamelloConfigLoader _configLoader;
        private PamelloModulesLoader _modulesLoader;
        private PamelloServerLoader _serverLoader;
        
        private readonly TaskCompletionSource _consoloniaCreated = new();
        
        public ConsoloniaApp Consolonia { get; set; } = null!;
        public WebApplication Asp { get; set; } = null!;
        
        public static void Main(string[] args) => new Program().ConsoloniaStartup(args);

        public void ConsoloniaStartup(string[] args) {
            var logger = new PamelloLogger();
            Output.Logger = logger;
            
            _configLoader = new PamelloConfigLoader();
            _configLoader.Load();
        
            _configLoader.InitType(typeof(ServerConfig), "Server");

            AppBuilder consoloniaBuilder;
            
            if (ServerConfig.Root.UseConsolonia) {
                consoloniaBuilder = AppBuilder.Configure<ConsoloniaApp>()
                    .UseConsolonia()
                    .UseAutoDetectedConsole()
                    .LogToException();

                consoloniaBuilder.AfterSetup(builder => {
                    Consolonia = (ConsoloniaApp)builder.Instance!;
                    _consoloniaCreated.SetResult();
                });
            }
            else {
                MainAsync(args).Wait();
                return;
            }
            
            var asp = Task.Run(async () => {
                try {
                    await MainAsync(args);
                }
                catch (Exception x) {
                    Output.Write($"Server Thread Crushed\n{x}", ELogLevel.Error);
                }
            });
            
            consoloniaBuilder.StartWithConsoleLifetime(args);
        }
        public async Task MainAsync(string[] args) {
            if (ServerConfig.Root.UseConsolonia) {
                await _consoloniaCreated.Task;
                await Consolonia.Started.Task;
                await Consolonia.LogScreen.LoadingCompleted.Task;
            }

            var aspBuilder = WebApplication.CreateBuilder(args);
            
            _serverLoader = new PamelloServerLoader(_configLoader);
            _modulesLoader = new PamelloModulesLoader(_configLoader);
            
            _serverLoader.Load();
            _modulesLoader.Load();
            
            if (!_modulesLoader.EnsureDependenciesAreSatisfied()) return;
            
            _serverLoader.ConfigureAssemblyServices(aspBuilder.Services);
            _modulesLoader.Configure(aspBuilder.Services);
            
            if (!EnsureServicesIsImplemented(aspBuilder.Services)) return;
            
            _serverLoader.ConfigureApiServices(aspBuilder.Services);
            
            aspBuilder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
            aspBuilder.WebHost.UseUrls($"{ServerConfig.Root.Host}");
            
            aspBuilder.Services.AddSingleton(aspBuilder.Services);

            aspBuilder.Services.AddSingleton(Output.Logger);
            
            Asp = aspBuilder.Build();
            
            _services = Asp.Services;

            _serverLoader.StartupAssemblyServices(Asp.Services);
            
            var types = (AssemblyTypeResolver)_services.GetRequiredService<IAssemblyTypeResolver>();
            types.LoadModules(_modulesLoader, _services);
            
            ((PamelloLogger)Output.Logger).SetServices(_services);
            
            var consolonia = _services.GetRequiredService<ConsoloniaService>();
            consolonia.SetApp(Consolonia);
            
            //Consolonia.SetMainScreen();

            foreach (var stage in Enum.GetValues<ELoadingStage>()) {
                try {
                    await _modulesLoader.StartupStage(Asp.Services, stage);
                }
                catch (ModuleStartupException x) {
                    Output.Write($"Module [{x.Module.Author}/{x.Module.Name}] failed to start: {x.Message}");
                    return;
                }
            }
        
            await StartupApp();
        }

        private bool EnsureServicesIsImplemented(IServiceCollection services) {
            var requiredServiceTypes = typeof(IPamelloService).Assembly.GetTypes()
                .Where(i => i != typeof(IPamelloService) && typeof(IPamelloService).IsAssignableFrom(i));
            
            var notConfiguredCount = 0;
            var serviceTypes = requiredServiceTypes as Type[] ?? requiredServiceTypes.ToArray();
            
            Output.Write($"Ensuring required services are implemented: ({serviceTypes.Count()} services)");
            foreach (var requiredServiceType in serviceTypes) {
                var service = services.FirstOrDefault(x => x.ServiceType == requiredServiceType);
                if (service is not null) continue;
                
                notConfiguredCount++;
                Output.Write($"{requiredServiceType.Name} is not implemented");
            }

            if (notConfiguredCount > 0) {
                Output.Write($"{notConfiguredCount} of required services are not implemented, aborting startup");
            }
            else {
                Output.Write($"All required services are implemented");
            }
            
            return notConfiguredCount == 0;
        }

        private async Task StartupApp() {
            Asp.MapHub<SignalHub>("/Signal");
            Asp.MapControllers();
            Asp.UseCors();

            var lifetime = Asp.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(OnStopping);
            lifetime.ApplicationStopped.Register(OnStop);
            lifetime.ApplicationStarted.Register(OnStart);

            await Asp.RunAsync();
        }

        private void OnStart() {
            Output.Write(Console.WindowWidth switch {
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
            
            var events = Asp.Services.GetRequiredService<IEventsService>();
            events.Invoke(null, new PamelloStarted() {
                Services = Asp.Services
            });
        }

        private void OnStopping() {
            Consolonia.SetLogScreen();
            
            Output.Write("\n---\nSTOPPING\n---");
            
            _modulesLoader.Shutdown(_services);
            _serverLoader.ShutdownAssemblyServices(_services);
        }
        private void OnStop() {
            Output.Write("\n---\nSTOPPED\n---");
        }
    }
}

//C17
