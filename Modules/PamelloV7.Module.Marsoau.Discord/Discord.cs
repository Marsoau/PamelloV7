using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using Discord.LibDave;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Modules;
using PamelloV7.Module.Marsoau.Discord.Config;
using PamelloV7.Module.Marsoau.Discord.Handlers;
using PamelloV7.Module.Marsoau.Discord.Services;

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
        Dave.SetLogSink((_, _, _, _) => { });
        
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

        await clients.Main.LoginAsync(TokenType.Bot, DiscordConfigOld.Root.Tokens.Main);
        await clients.Main.StartAsync();

        whenReady.Task.Wait();
        
        clients.LateStartup();
    }

    public async Task DiscordLog(LogMessage message) {
        //if ((int)message.Severity >= 2) return;
        
        Console.WriteLine($"[Discord {message.Severity} | message] {message.Message}");
        if (message.Exception is not null) Console.WriteLine($"[Discord {message.Severity} | exception] {message.Exception}");
    }
}
