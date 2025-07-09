using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PamelloV7.DAL;
using PamelloV7.Server.Config;
using PamelloV7.Server.Filters;
using PamelloV7.Server.Handlers;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Discord.Audio;
using PamelloV7.Core.Data;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Server.Database;
using PamelloV7.Server.Model.Audio.Modules.Basic;
using PamelloV7.Server.Model.Audio.Modules.Inputs;
using PamelloV7.Server.Plugins;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server
{
    public class Program
    {
        public static async Task Main(string[] args) => await new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
            StaticLogger.Log($"PamelloV7 {Assembly.GetExecutingAssembly().GetName().Version}");
            
            Console.OutputEncoding = Encoding.Unicode;

            var builder = WebApplication.CreateBuilder(args);
            var pluginLoader = new PamelloPluginLoader();
            
            pluginLoader.Load();
            
            builder.WebHost.UseUrls($"http://{PamelloServerConfig.Root.Host}");

            //ConfigureDatabaseServices(builder.Services);
            //ConfigurePamelloServices(builder.Services);
            //ConfigureApiServices(builder.Services);
            builder.Services.AddSingleton<IPamelloLogger, PamelloLogger>();
            builder.Services.AddSingleton<IDataAccessService, DataAccessService>();
            
            pluginLoader.Configure(builder.Services);
            
            var app = builder.Build();

            //await StartupDatabaseServices(app.Services);
            //await StartupPamelloServices(app.Services);
            
            app.Services.GetRequiredService<IDataAccessService>().Startup(app.Services);
            
            pluginLoader.Startup(app.Services);
            
            //await StartupApiServices(app);
        }

        private void ConfigureDatabaseServices(IServiceCollection services) {
        }

        private void ConfigurePamelloServices(IServiceCollection services) {
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

        private async Task StartupDatabaseServices(IServiceProvider services) {
        }

        private async Task StartupPamelloServices(IServiceProvider services) {
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

//C12
