using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Modules;
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
            AlwaysDownloadUsers = true
        };

        services.AddSingleton(new DiscordSocketClient(discordConfig));
        
        services.AddSingleton(s => new InteractionService(
            s.GetRequiredService<DiscordSocketClient>(),
            new InteractionServiceConfig()
        ));
    }
    
    public void Startup(IServiceProvider services) {
        var clients = services.GetRequiredService<DiscordClientService>();
        var interactionHandler = services.GetRequiredService<InteractionHandler>();

        var whenReady = new TaskCompletionSource();
        
        interactionHandler.LoadAsync().Wait();

        clients.Main.Log += DiscordLog;
        clients.Main.Ready += async () => {
            Console.WriteLine("Discord client ready");
            
            await interactionHandler.RegisterAsync();
            
            whenReady.SetResult();
        };

        clients.Main.LoginAsync(TokenType.Bot, DiscordConfig.Root.Tokens.Main).Wait();
        clients.Main.StartAsync().Wait();

        whenReady.Task.Wait();
    }

    public async Task DiscordLog(LogMessage message) {
        if ((int)message.Severity >= 4) return;
            
        Console.WriteLine($"[Discord {message.Severity} | message] {message.Message}");
        Console.WriteLine($"[Discord {message.Severity} | exception] {message.Exception}");
    }
}
