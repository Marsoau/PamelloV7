using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using Discord.LibDave;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Module.Marsoau.Discord.Config;
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
    }
    
    public async Task StartupAsync(IServiceProvider services) {
        Dave.SetLogSink((_, _, _, _) => { });
        
        var clients = services.GetRequiredService<DiscordClientService>();
        var interactionHandler = services.GetRequiredService<InteractionHandler>();

        var whenReady = new TaskCompletionSource();

        clients.Main.Log += DiscordLog;
        clients.Main.Ready += async () => {
            Output.Write("Discord client ready");
            
            whenReady.SetResult();
        };

        await clients.Main.LoginAsync(TokenType.Bot, DiscordConfig.Root.Tokens.Main);
        await clients.Main.StartAsync();

        whenReady.Task.Wait();
        
        //var commands = await clients.Main.GetGlobalApplicationCommandsAsync();
        //Task.WaitAll(commands.Select(command => command.DeleteAsync()));
            
        await interactionHandler.RegisterAsync();
        
        clients.LateStartup();
    }

    public async Task DiscordLog(LogMessage message) {
        //if ((int)message.Severity >= 2) return;
        
        Output.Write($"[Discord {message.Severity} | message] {message.Message}");
        if (message.Exception is not null) Output.Write($"[Discord {message.Severity} | exception] {message.Exception}");
    }
}
