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
using PamelloV7.Server.Model.Audio.Modules.Basic;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server
{
    public class Program
    {
        private WebApplicationBuilder builder;
        private WebApplication app;

        public static async Task Main(string[] args) => await new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
            Console.OutputEncoding = Encoding.Unicode;

            builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls($"http://{PamelloServerConfig.Root.Host}");

            ConfigureDatabaseServices();
            ConfigureDiscordServices();
            ConfigurePamelloServices();
            ConfigureAPIServices();

            app = builder.Build();

            await StartupDatabaseServices();
            await StartupDiscordServices();
            await StartupPamelloServices();
            await StartupAPIServices();
        }

        private void ConfigureDatabaseServices() {
            builder.Services.AddTransient<DatabaseContext>();
        }

        private void ConfigureDiscordServices() {
            var discordConfig = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

            builder.Services.AddSingleton(new DiscordSocketClient(discordConfig));
            for (int i = 0; i < PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
                builder.Services.AddKeyedSingleton($"Speaker-{i + 1}", new DiscordSocketClient(discordConfig));
            }

            builder.Services.AddSingleton(services => new InteractionService(
                services.GetRequiredService<DiscordSocketClient>(),
                new InteractionServiceConfig()
            ));
            builder.Services.AddSingleton<InteractionHandler>();

            builder.Services.AddSingleton<DiscordClientService>();
        }

        private void ConfigurePamelloServices() {
            builder.Services.AddSingleton<PamelloEventsService>();

            builder.Services.AddSingleton<YoutubeInfoService>();
            builder.Services.AddSingleton<YoutubeDownloadService>();

            builder.Services.AddSingleton<PamelloUserRepository>();
            builder.Services.AddSingleton<PamelloSongRepository>();
            builder.Services.AddSingleton<PamelloEpisodeRepository>();
            builder.Services.AddSingleton<PamelloPlaylistRepository>();

            builder.Services.AddSingleton<PamelloPlayerRepository>();
            builder.Services.AddSingleton<PamelloSpeakerRepository>();

            builder.Services.AddSingleton<UserAuthorizationService>();
        }

        private void ConfigureAPIServices() {
            builder.Services.AddControllers(config => config.Filters.Add<PamelloExceptionFilter>());
            builder.Services.AddHttpClient();

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", builder => {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        private async Task StartupDatabaseServices() {
            app.Services.GetRequiredService<DatabaseContext>();
        }

        private async Task StartupDiscordServices() {
            var discordClients = app.Services.GetRequiredService<DiscordClientService>();

            discordClients.SubscriveToEvents();

            var interactionService = app.Services.GetRequiredService<InteractionService>();
            var interactionHandler = app.Services.GetRequiredService<InteractionHandler>();

            var youtube = app.Services.GetRequiredService<YoutubeInfoService>();

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

            await discordClients.MainClient.LoginAsync(TokenType.Bot,
                PamelloServerConfig.Root.Discord.Tokens.MainBotToken);
            await discordClients.MainClient.StartAsync();

            await mainDiscordReady.Task;

            for (int i = 0; i < PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens.Length; i++) {
                await discordClients.DiscordClients[i + 1].LoginAsync(TokenType.Bot,
                    PamelloServerConfig.Root.Discord.Tokens.SpeakerTokens[i]);
                await discordClients.DiscordClients[i + 1].StartAsync();
            }
        }

        private async Task StartupPamelloServices() {
            var events = app.Services.GetRequiredService<PamelloEventsService>();

            var users = app.Services.GetRequiredService<PamelloUserRepository>();
            var songs = app.Services.GetRequiredService<PamelloSongRepository>();
            var episodes = app.Services.GetRequiredService<PamelloEpisodeRepository>();
            var playlists = app.Services.GetRequiredService<PamelloPlaylistRepository>();

            var players = app.Services.GetRequiredService<PamelloPlayerRepository>();
            var speakers = app.Services.GetRequiredService<PamelloSpeakerRepository>();

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

            app.MapControllers();
            app.UseCors("AllowSpecificOrigin");

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopped.Register(OnStop);
            lifetime.ApplicationStarted.Register(OnStart);

            await app.RunAsync();
        }

        private async void OnStart() {
            var users = app.Services.GetRequiredService<PamelloUserRepository>();

            var user = users.GetRequired(1);
            var player = await user.Commands.PlayerCreate("Test");

            var model = new AudioModel();

            model.AddModule(player);

            Console.WriteLine("end model");

            /*
            var audio = new PamelloAudio(app.Services, songs.GetRequired(1));
            var pump = new AudioPump();
            var copy = new AudioCopy();
            var speaker = await user.Commands.SpeakerInternetConnect("test", true);
            var speaker2 = await user.Commands.SpeakerInternetConnect("test2", true);
            var speaker3 = await user.Commands.SpeakerInternetConnect("test3", true);

            Console.WriteLine(await audio.TryInitialize());
            pump.InitModule();
            copy.InitModule();

            pump.Input.ConnectBack(audio.Output);
            pump.Output.ConnectFront(copy.Input);

            copy.CreateOutput().ConnectFront(speaker.Input);
            copy.CreateOutput().ConnectFront(speaker2.Input);
            copy.CreateOutput().ConnectFront(speaker3.Input);
            while (true) {
                await audio.NextBytes(pair);
                await player.Speakers.BroadcastBytes(player, pair);
            }
            var pair = new byte[2];
            await audio.Output.Pull(pair);
            Console.WriteLine($"sample: {pair[0]}, {pair[1]}");
            await audio.NextBytes(pair);
            Console.WriteLine($"sample: {pair[0]}, {pair[1]}");

            Task.Delay(1000).Wait();

            _ = pump.Start();
            */
        }

        private void OnStop() {
            Console.WriteLine("STOPPING");
        }
    }
}
