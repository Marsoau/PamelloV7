using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Modules;
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
        return;
        var discord = services.GetRequiredService<DiscordSocketClient>();

        var whenReady = new TaskCompletionSource();

        discord.Log += async message => {
            Console.WriteLine($"[Discord Log] {message}");
        };

        discord.Ready += async () => {
            whenReady.SetResult();
        };

        discord.LoginAsync(TokenType.Bot, DiscordConfig.Root.Tokens.Main).Wait();
        discord.StartAsync().Wait();

        whenReady.Task.Wait();
    }
}
