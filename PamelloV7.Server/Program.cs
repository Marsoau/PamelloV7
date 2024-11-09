using Discord;
using Discord.WebSocket;
using PamelloV7.DAL;
using PamelloV7.Server.Config;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;

namespace PamelloV7.Server
{
    public class Program
    {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<PamelloServerConfig>();

            ConfigureDatabaseServices(builder.Services);
            ConfigureDiscordServices(builder.Services);
            ConfigurePamelloServices(builder.Services);
            ConfigureAPIServices(builder.Services);

            var app = builder.Build();

            await StartupDatabaseServices(app.Services);
            await StartupDiscordServices(app.Services);
            await StartupPamelloServices(app.Services);
            await StartupAPIServices(app);
        }

        private static void ConfigureDatabaseServices(IServiceCollection services) {
            services.AddSingleton<DatabaseContext>();
        }

        private static void ConfigureDiscordServices(IServiceCollection services) {
            var discordConfig = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

            services.AddSingleton(s => new DiscordSocketClient(discordConfig));

            services.AddSingleton<DiscordClientService>();
        }

        private static void ConfigurePamelloServices(IServiceCollection services) {
            services.AddSingleton<PamelloUserRepository>();
        }

        private static void ConfigureAPIServices(IServiceCollection services) {
            services.AddControllers();
        }

        private static async Task StartupDatabaseServices(IServiceProvider services) {
            services.GetRequiredService<DatabaseContext>();
        }

        private static async Task StartupDiscordServices(IServiceProvider services) {
            var discordClients = services.GetRequiredService<DiscordClientService>();
            var config = services.GetRequiredService<PamelloServerConfig>();

            var discordReady = new TaskCompletionSource();

            discordClients.MainClient.Log += async (message) => {
                Console.WriteLine(message);
            };

            discordClients.MainClient.Ready += async () => {
                discordReady.SetResult();
            };

            await discordClients.MainClient.LoginAsync(TokenType.Bot, config.MainBotToken);
            await discordClients.MainClient.StartAsync();

            await discordReady.Task;
        }

        private static async Task StartupPamelloServices(IServiceProvider services) {
            var users = services.GetRequiredService<PamelloUserRepository>();
        }

        private static async Task StartupAPIServices(WebApplication app) {
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
