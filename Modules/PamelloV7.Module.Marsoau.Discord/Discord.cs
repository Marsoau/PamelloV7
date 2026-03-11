using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using Discord.LibDave;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Modules;
using PamelloV7.Module.Marsoau.Discord.Handlers;
using PamelloV7.Module.Marsoau.Discord.Services;
using DiscordConfig = PamelloV7.Module.Marsoau.Discord.Config.DiscordConfig;

namespace PamelloV7.Module.Marsoau.Discord;

public class Discord : IPamelloModule
{
    public string Name => "Discord";
    public string Author => "Marsoau";
    public string Description => "Discord user platform integration";
    public ELoadingStage Stage => ELoadingStage.Default;

    public void Configure(IServiceCollection services) {
        Console.WriteLine("Configure start");
        var discordConfig = new DiscordSocketConfig() {
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true,
            EnableVoiceDaveEncryption = true,
            LogLevel = LogSeverity.Error
        };

        services.AddSingleton(new DiscordSocketClient(discordConfig));
        services.AddSingleton(s => new InteractionService(
            s.GetRequiredService<DiscordSocketClient>(),
            new InteractionServiceConfig()
        ));
        Console.WriteLine("Configure end");
    }
    
    public async Task StartupAsync(IServiceProvider services) {
        Console.WriteLine("Startup startr");
        var dependencies = services.GetRequiredService<IDependenciesService>();
        SetOwnDependenciesPaths(dependencies);
        
        //Dave.SetLogSink((_, _, _, _) => { });
        
        var clients = services.GetRequiredService<DiscordClientService>();
        var interactionHandler = services.GetRequiredService<InteractionHandler>();
        var modalHandler = services.GetRequiredService<ModalSubmissionHandler>();

        var whenReady = new TaskCompletionSource();
        
        await interactionHandler.LoadAsync();
        modalHandler.Load();

        clients.Main.Log += DiscordLog;
        clients.Main.Ready += async () => {
            Console.WriteLine("Discord client ready");
            
            await interactionHandler.RegisterAsync();
            
            whenReady.SetResult();
        };

        await clients.Main.LoginAsync(TokenType.Bot, DiscordConfig.Root.Tokens.Main);
        await clients.Main.StartAsync();

        whenReady.Task.Wait();
        
        clients.LateStartup();
    }

    public void SetOwnDependenciesPaths(IDependenciesService dependencies) {
        var sodium = dependencies.ResolveRequired("sodium");
        var opus = dependencies.ResolveRequired("opus");
        var dave = dependencies.ResolveRequired("dave");

        NativeLibrary.SetDllImportResolver(typeof(DiscordSocketClient).Assembly, (libraryName, assembly, searchPath) => {
            switch (libraryName) {
                case "libsodium" or "sodium":
                    Console.WriteLine("Loading libsodium from our path");
                    Console.WriteLine(sodium.IsInstalled);
                    if (!sodium.IsInstalled) return IntPtr.Zero;
                    
                    return NativeLibrary.Load(sodium.GetFile().FullName);
                case "opus" or "libopus":
                    Console.WriteLine("Loading opus from our path");
                    Console.WriteLine(opus.IsInstalled);
                    if (!opus.IsInstalled) return IntPtr.Zero;
                    
                    return NativeLibrary.Load(opus.GetFile().FullName);
                case "dave" or "libdave":
                    Console.WriteLine("Loading dave from our path");
                    Console.WriteLine(dave.IsInstalled);
                    if (!dave.IsInstalled) return IntPtr.Zero;
                    
                    return NativeLibrary.Load(dave.GetFile().FullName);
                default:
                    return IntPtr.Zero;
            }
        });
    }

    public async Task DiscordLog(LogMessage message) {
        //if ((int)message.Severity >= 2) return;
        
        Console.WriteLine($"[Discord {message.Severity} | message] {message.Message}");
        if (message.Exception is not null) Console.WriteLine($"[Discord {message.Severity} | exception] {message.Exception}");
    }
}
