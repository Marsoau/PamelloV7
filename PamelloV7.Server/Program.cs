using Discord;
using Discord.Audio;
using Discord.Interactions;
using Discord.WebSocket;
using PamelloV7.DAL;
using PamelloV7.Server.Config;
using PamelloV7.Server.Handlers;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;
using System.Diagnostics;
using System.Text;

namespace PamelloV7.Server
{
    public class Program
    {
        public static async Task Main(string[] args) {
            Console.OutputEncoding = Encoding.Unicode;

            var builder = WebApplication.CreateBuilder(args);

            ConfigureDatabaseServices(builder.Services);
            ConfigureDiscordServices(builder.Services);
            ConfigurePamelloServices(builder.Services);
            ConfigureAPIServices(builder.Services);

            var app = builder.Build();

            await StartupDatabaseServices(app.Services);
            await StartupPamelloServices(app.Services);
            await StartupDiscordServices(app.Services);
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

            services.AddSingleton(new DiscordSocketClient(discordConfig));
            for (int i = 0; i < PamelloServerConfig.SpeakerTokens.Length; i++) {
                services.AddKeyedSingleton($"Speaker-{i + 1}", new DiscordSocketClient(discordConfig));
            }
            
            services.AddSingleton(services => new InteractionService(
                services.GetRequiredService<DiscordSocketClient>(),
                new InteractionServiceConfig()
            ));
            services.AddSingleton<InteractionHandler>();

            services.AddSingleton<DiscordClientService>();
        }

        private static void ConfigurePamelloServices(IServiceCollection services) {
            services.AddSingleton<PamelloEventsService>();

            services.AddSingleton<YoutubeInfoService>();
            services.AddSingleton<YoutubeDownloadService>();

            services.AddSingleton<PamelloUserRepository>();
            services.AddSingleton<PamelloSongRepository>();
            services.AddSingleton<PamelloEpisodeRepository>();
            services.AddSingleton<PamelloPlaylistRepository>();
            services.AddSingleton<PamelloPlayerRepository>();

            services.AddSingleton<PamelloSpeakerService>();
            services.AddSingleton<UserAuthorizationService>();
        }

        private static void ConfigureAPIServices(IServiceCollection services) {
            services.AddControllers();
            services.AddHttpClient();
        }

        private static async Task StartupDatabaseServices(IServiceProvider services) {
            services.GetRequiredService<DatabaseContext>();
        }

        private static async Task StartupDiscordServices(IServiceProvider services) {
            var discordClients = services.GetRequiredService<DiscordClientService>();

            discordClients.SubscriveToEvents();

            var users = services.GetRequiredService<PamelloUserRepository>();
            var songs = services.GetRequiredService<PamelloSongRepository>();
            var players = services.GetRequiredService<PamelloPlayerRepository>();
            var speakers = services.GetRequiredService<PamelloSpeakerService>();
            var downloader = services.GetRequiredService<YoutubeDownloadService>();

            var interactionService = services.GetRequiredService<InteractionService>();
            var interactionHandler = services.GetRequiredService<InteractionHandler>();

            var youtube = services.GetRequiredService<YoutubeInfoService>();

            await interactionHandler.InitializeAsync();

            var mainDiscordReady = new TaskCompletionSource();

            discordClients.MainClient.Log += async (message) => {
                Console.WriteLine($">discord<: {message}");
            };

            discordClients.MainClient.Ready += async () => {
                mainDiscordReady.SetResult();

                var guild = discordClients.MainClient.GetGuild(1304142495453548646);
                await interactionService.RegisterCommandsToGuildAsync(guild.Id);
            };
            discordClients.DiscordClients[1].Log += async (message) => {
                Console.WriteLine($">speaker<: {message}");
            };
            discordClients.DiscordClients[1].Ready += async () => {
                Console.WriteLine("speaker ready");
            };

            await discordClients.MainClient.LoginAsync(TokenType.Bot, PamelloServerConfig.MainBotToken);
            await discordClients.MainClient.StartAsync();

            await mainDiscordReady.Task;

            for (int i = 0; i < PamelloServerConfig.SpeakerTokens.Length; i++) {
                await discordClients.DiscordClients[i + 1].LoginAsync(TokenType.Bot, PamelloServerConfig.SpeakerTokens[i]);
                await discordClients.DiscordClients[i + 1].StartAsync();
            }
        }

        private static async Task StartupPamelloServices(IServiceProvider services) {
            var events = services.GetRequiredService<PamelloEventsService>();

            events.OnUserCreated += async (user) => {
                Console.WriteLine($"Created new user {user.Id}");
            };
            events.OnUserLoaded += async (user) => {
                Console.WriteLine($"Loaded {user}");
            };

            events.OnDownloadStart += (song) => {
                Console.WriteLine($"Started download of \"{song}\"");
            };
            events.OnDownloadEnd += (song, result) => {
                Console.WriteLine($"Ended download of \"{song}\" with result \"{result}\"");
            };

            var users = services.GetRequiredService<PamelloUserRepository>();
            var songs = services.GetRequiredService<PamelloSongRepository>();
            var episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            var playlists = services.GetRequiredService<PamelloPlaylistRepository>();
            var players = services.GetRequiredService<PamelloPlayerRepository>();

            users.InitServices();
            songs.InitServices();
            episodes.InitServices();
            playlists.InitServices();
            players.InitServices();
        }

        private static async Task StartupAPIServices(WebApplication app) {
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
