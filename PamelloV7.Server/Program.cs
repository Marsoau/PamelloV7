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
using System.Text;
using Discord.Audio;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Model.Audio.Modules.Basic;
using PamelloV7.Server.Model.Audio.Modules.Inputs;
using PamelloV7.Server.Plugins;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server
{
    public class Program
    {
        private WebApplicationBuilder _builder;
        private WebApplication _app;

        private PamelloPluginLoader _pluginLoader;

        public static async Task Main(string[] args) => await new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
            Console.OutputEncoding = Encoding.Unicode;

            _builder = WebApplication.CreateBuilder(args);
            _pluginLoader = new PamelloPluginLoader();
            
            _pluginLoader.Load();
            
            _builder.WebHost.UseUrls($"http://{PamelloServerConfig.Root.Host}");

            ConfigureDatabaseServices();
            ConfigureDiscordServices();
            ConfigurePamelloServices();
            ConfigureAPIServices();
            
            _pluginLoader.Configure(_builder.Services);
            
            _app = _builder.Build();
            
            _pluginLoader.Startup(_app.Services);

            await StartupDatabaseServices();
            await StartupDiscordServices();
            await StartupPamelloServices();
            await StartupAPIServices();
        }

        private void ConfigureDatabaseServices() {
            _builder.Services.AddTransient(_ => new DatabaseContext(PamelloServerConfig.Root.DataPath));
        }

        private void ConfigureDiscordServices() {
            var discordConfig = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

            _builder.Services.AddSingleton(new DiscordSocketClient(discordConfig));
            for (int i = 0; i < PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
                _builder.Services.AddKeyedSingleton($"Speaker-{i + 1}", new DiscordSocketClient(discordConfig));
            }

            _builder.Services.AddSingleton(services => new InteractionService(
                services.GetRequiredService<DiscordSocketClient>(),
                new InteractionServiceConfig()
            ));
            _builder.Services.AddSingleton<InteractionHandler>();

            _builder.Services.AddSingleton<DiscordClientService>();
        }

        private void ConfigurePamelloServices() {
            _builder.Services.AddSingleton<PamelloEventsService>();

            _builder.Services.AddSingleton<YoutubeInfoService>();
            _builder.Services.AddSingleton<YoutubeDownloadService>();

            _builder.Services.AddSingleton<IPamelloUserRepository, PamelloUserRepository>();
            _builder.Services.AddSingleton<IPamelloSongRepository, PamelloSongRepository>();
            _builder.Services.AddSingleton<IPamelloEpisodeRepository, PamelloEpisodeRepository>();
            _builder.Services.AddSingleton<IPamelloPlaylistRepository, PamelloPlaylistRepository>();

            _builder.Services.AddSingleton<IPamelloPlayerRepository, PamelloPlayerRepository>();
            _builder.Services.AddSingleton<IPamelloSpeakerRepository, PamelloSpeakerRepository>();
            
            _builder.Services.AddSingleton<AudioModel>();
        }

        private void ConfigureAPIServices() {
            _builder.Services.AddControllers(config => config.Filters.Add<PamelloExceptionFilter>());
            _builder.Services.AddHttpClient();

            _builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", builder => {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            
            _builder.Services.AddHttpContextAccessor();
        }

        private async Task StartupDatabaseServices() {
            _app.Services.GetRequiredService<DatabaseContext>();
        }

        private async Task StartupDiscordServices() {
            var discordClients = _app.Services.GetRequiredService<DiscordClientService>();

            discordClients.SubscriveToEvents();

            var interactionService = _app.Services.GetRequiredService<InteractionService>();
            var interactionHandler = _app.Services.GetRequiredService<InteractionHandler>();

            var youtube = _app.Services.GetRequiredService<YoutubeInfoService>();

            await interactionHandler.InitializeAsync();

            var mainDiscordReady = new TaskCompletionSource();

            discordClients.MainClient.Log += async (message) => {
                //Console.WriteLine($">discord<: {message}");
            };

            discordClients.MainClient.Ready += async () => {
                mainDiscordReady.SetResult();
                Console.WriteLine($"Main discord client {discordClients.MainClient.CurrentUser.Username} is ready");;

                if (PamelloServerConfig.Root.Discord.Commands.GlobalRegistration)
                {
                    await interactionService.RegisterCommandsGloballyAsync();
                }
                else foreach (var guildId in PamelloServerConfig.Root.Discord.Commands.GuildsIds)
                {
                    var guild = discordClients.MainClient.GetGuild(guildId);
                    await interactionService.RegisterCommandsToGuildAsync(guild.Id);
                }
            };

            foreach (var speakerClient in discordClients.DiscordClients.Skip(1)) {
                speakerClient.Ready += () => SpeakerClient_OnReady(speakerClient);
            }
            
            await discordClients.MainClient.LoginAsync(TokenType.Bot,
                PamelloServerConfig.Root.Discord.Tokens.MainBotToken);
            await discordClients.MainClient.StartAsync();

            await mainDiscordReady.Task;

            for (var i = 0; i < PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
                await discordClients.DiscordClients[i + 1].LoginAsync(TokenType.Bot,
                    PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens[i]);
                await discordClients.DiscordClients[i + 1].StartAsync();
            }
        }

        private async Task SpeakerClient_OnReady(DiscordSocketClient client) {
            Console.WriteLine($"Speaker discord client {client.CurrentUser.Username} is ready");
        }

        private async Task StartupPamelloServices() {
            var events = _app.Services.GetRequiredService<PamelloEventsService>();

            var users = _app.Services.GetRequiredService<IPamelloUserRepository>();
            var songs = _app.Services.GetRequiredService<IPamelloSongRepository>();
            var episodes = _app.Services.GetRequiredService<IPamelloEpisodeRepository>();
            var playlists = _app.Services.GetRequiredService<IPamelloPlaylistRepository>();

            var players = _app.Services.GetRequiredService<IPamelloPlayerRepository>();
            var speakers = _app.Services.GetRequiredService<IPamelloSpeakerRepository>();

            songs.BeforeLoading += () => { DatabaseEntityRepository_BeforeLoading("Loading songs"); };
            episodes.BeforeLoading += () => { DatabaseEntityRepository_BeforeLoading("Loading episodes"); };
            playlists.BeforeLoading += () => { DatabaseEntityRepository_BeforeLoading("Loading playlists"); };
            users.BeforeLoading += () => { DatabaseEntityRepository_BeforeLoading("Loading users"); };

            songs.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;
            episodes.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;
            playlists.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;
            users.OnLoadingProgress += DatabaseEntityRepository_OnLoadingProgress;

            songs.OnLoaded += DatabaseEntityRepository_OnLoaded;
            episodes.OnLoaded += DatabaseEntityRepository_OnLoaded;
            playlists.OnLoaded += DatabaseEntityRepository_OnLoaded;
            users.OnLoaded += DatabaseEntityRepository_OnLoaded;

            songs.BeforeInit += () => { DatabaseEntityRepository_BeforeLoading("Initializing songs"); };
            episodes.BeforeInit += () => { DatabaseEntityRepository_BeforeLoading("Initializing episodes"); };
            playlists.BeforeInit += () => { DatabaseEntityRepository_BeforeLoading("Initializing playlists"); };
            users.BeforeInit += () => { DatabaseEntityRepository_BeforeLoading("Initializing users"); };

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

        private void DatabaseEntityRepository_OnLoaded() {
            Console.WriteLine("\nDone");
        }

        private void DatabaseEntityRepository_BeforeLoading(string name) {
            Console.WriteLine($"{name}");
        }

        private void DatabaseEntityRepository_OnLoadingProgress(int loaded, int total) {
            Console.Write($"\r[{loaded}/{total}] {((double)loaded / total) * 100}%                  ");
        }

        private async Task StartupAPIServices() {
            //app.UseHttpsRedirection();

            _app.MapControllers();
            _app.UseCors("AllowSpecificOrigin");

            var lifetime = _app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(OnStopping);
            lifetime.ApplicationStopped.Register(OnStop);
            lifetime.ApplicationStarted.Register(OnStart);

            await _app.RunAsync();
        }

        private void OnStart() {
            Console.WriteLine("STARTED");
        }

        private void OnStop() {
            Console.WriteLine("STOP");
        }
        private void OnStopping() {
            Console.WriteLine("STOPPING");
            
            var events = _app.Services.GetRequiredService<PamelloEventsService>();
            var audio = _app.Services.GetRequiredService<AudioModel>();
            
            events.Dispose();
            audio.Dispose();
        }
    }
}

//C12
