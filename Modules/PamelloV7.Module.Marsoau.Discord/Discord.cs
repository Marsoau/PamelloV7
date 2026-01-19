using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Modules;
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
    }
    
    public void Startup(IServiceProvider services) {
        var clients = services.GetRequiredService<DiscordClientService>();

        var whenReady = new TaskCompletionSource();

        clients.Main.Log += async message => {
            Console.WriteLine($"[Discord Log] {message}");
        };

        clients.Main.Ready += async () => {
            whenReady.SetResult();
        };

        clients.Main.LoginAsync(TokenType.Bot, DiscordConfig.Root.Tokens.Main).Wait();
        clients.Main.StartAsync().Wait();

        whenReady.Task.Wait();

        var duser = clients.GetDiscordUser(544933092503060509);
        Console.WriteLine($"Discord user: {duser?.GlobalName}");
    }
}
