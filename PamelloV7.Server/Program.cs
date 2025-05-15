using Discord;
using Discord.Audio;
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
using System.Text;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server
{
    public class Program
    {
        public static async Task Main(string[] args) {
            Console.OutputEncoding = Encoding.Unicode;

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls($"http://{PamelloServerConfig.Root.Host}");

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
            services.AddTransient<DatabaseContext>();
        }

        private static void ConfigureDiscordServices(IServiceCollection services) {
            var discordConfig = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

            services.AddSingleton(new DiscordSocketClient(discordConfig));
            for (int i = 0; i < PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
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
            services.AddSingleton<PamelloSpeakerRepository>();

            services.AddSingleton<UserAuthorizationService>();
        }

        private static void ConfigureAPIServices(IServiceCollection services) {
            services.AddControllers(config => config.Filters.Add<PamelloExceptionFilter>());
            services.AddHttpClient();

			services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", builder => {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        private static async Task StartupDatabaseServices(IServiceProvider services) {
            services.GetRequiredService<DatabaseContext>();
        }

        private static async Task StartupDiscordServices(IServiceProvider services) {
            var discordClients = services.GetRequiredService<DiscordClientService>();

            discordClients.SubscriveToEvents();

            var interactionService = services.GetRequiredService<InteractionService>();
            var interactionHandler = services.GetRequiredService<InteractionHandler>();

            var youtube = services.GetRequiredService<YoutubeInfoService>();

            await interactionHandler.InitializeAsync();

            var mainDiscordReady = new TaskCompletionSource();

            discordClients.MainClient.Log += async (message) => {
                //Console.WriteLine($">discord<: {message}");
            };

            discordClients.MainClient.Ready += async () => {
                mainDiscordReady.SetResult();

                var guild = discordClients.MainClient.GetGuild(1304142495453548646);
                //await guild.DeleteApplicationCommandsAsync();
                await interactionService.RegisterCommandsToGuildAsync(guild.Id);
            };
            discordClients.DiscordClients[1].Log += async (message) => {
                //Console.WriteLine($">speaker<: {message}");
            };
            discordClients.DiscordClients[1].Ready += async () => {
                //Console.WriteLine("speaker ready");
            };

            await discordClients.MainClient.LoginAsync(TokenType.Bot, PamelloServerConfig.Root.Discord.Tokens.MainBotToken);
            await discordClients.MainClient.StartAsync();

            await mainDiscordReady.Task;

            for (int i = 0; i < PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
                await discordClients.DiscordClients[i + 1].LoginAsync(TokenType.Bot, PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens[i]);
                await discordClients.DiscordClients[i + 1].StartAsync();
            }
        }

        private static async Task StartupPamelloServices(IServiceProvider services) {
            var events = services.GetRequiredService<PamelloEventsService>();

            var users = services.GetRequiredService<PamelloUserRepository>();
            var songs = services.GetRequiredService<PamelloSongRepository>();
            var episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            var playlists = services.GetRequiredService<PamelloPlaylistRepository>();
            
            var players = services.GetRequiredService<PamelloPlayerRepository>();
            var speakers = services.GetRequiredService<PamelloSpeakerRepository>();

            songs.BeforeLoading += () => {
                DatabaseEntityRepository_BeforeLoading("Loading songs");
            };
            episodes.BeforeLoading += () => {
                DatabaseEntityRepository_BeforeLoading("Loading episodes");
            };
            playlists.BeforeLoading += () => {
                DatabaseEntityRepository_BeforeLoading("Loading playlists");
            };
            users.BeforeLoading += () => {
                DatabaseEntityRepository_BeforeLoading("Loading users");
            };

            songs.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;
            episodes.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;
            playlists.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;
            users.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;

            songs.OnLoaded += DatabaseEntityRepository_OnLoaded;
            episodes.OnLoaded += DatabaseEntityRepository_OnLoaded;
            playlists.OnLoaded += DatabaseEntityRepository_OnLoaded;
            users.OnLoaded += DatabaseEntityRepository_OnLoaded;

            songs.BeforeInit += () => {
                DatabaseEntityRepository_BeforeLoading("Initializing songs");
            };
            episodes.BeforeInit += () => {
                DatabaseEntityRepository_BeforeLoading("Initializing episodes");
            };
            playlists.BeforeInit += () => {
                DatabaseEntityRepository_BeforeLoading("Initializing playlists");
            };
            users.BeforeInit += () => {
                DatabaseEntityRepository_BeforeLoading("Initializing users");
            };

            songs.OnInitProgress += DatabaseEntityRepository_OnLoadingProgress;
            episodes.OnInitProgress += DatabaseEntityRepository_OnLoadingProgress;
            playlists.OnInitProgress += DatabaseEntityRepository_OnLoadingProgress;
            users.OnInitProgress += DatabaseEntityRepository_OnLoadingProgress;

            songs.OnInit += DatabaseEntityRepository_OnLoaded;
            episodes.OnInit += DatabaseEntityRepository_OnLoaded;
            playlists.OnInit += DatabaseEntityRepository_OnLoaded;
            users.OnInit += DatabaseEntityRepository_OnLoaded;

            users.InitServices();
            songs.InitServices();
            episodes.InitServices();
            playlists.InitServices();
            
            players.InitServices();
            speakers.InitServices();

            await songs.LoadAllAsync();
            await episodes.LoadAllAsync();
            await playlists.LoadAllAsync();
            await users.LoadAllAsync();

            await songs.InitAllAsync();
            await episodes.InitAllAsync();
            await playlists.InitAllAsync();
            await users.InitAllAsync();
        }

        private static void DatabaseEntityRepository_OnLoaded() {
            Console.WriteLine("\nDone");
        }
        private static void DatabaseEntityRepository_BeforeLoading(string name) {
            Console.WriteLine($"{name}");
        }
        private static void DatabaseEntityRepository_OnLoadingProgress(int loaded, int total) {
            Console.Write($"\r[{loaded}/{total}] {((double)loaded / total) * 100}%                  ");
        }

        private static async Task StartupAPIServices(WebApplication app) {
            //app.UseHttpsRedirection();

            app.MapControllers();
            app.UseCors("AllowSpecificOrigin");

            await app.RunAsync();
        }
    }
}
