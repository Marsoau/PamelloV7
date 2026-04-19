using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Logger;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class DiscordClientService : IPamelloService
{
    public GatewayClient[] Clients = [];
    public GatewayClient Main => Clients.FirstOrDefault() ?? throw new PamelloException("Main client is not registered yet");

    public IReadOnlyList<ApplicationEmoji> Emojis { get; private set; } = [];

    public void Startup(IServiceProvider services) {
        var main = new GatewayClient(new BotToken(NetCordConfig.Root.Tokens.Main), new GatewayClientConfiguration() {
            Logger = new NetCordLogger(),
            RestClientConfiguration = new RestClientConfiguration() {
                Version = ApiVersion.V10,
            }
        });
        
        var speakerClients = NetCordConfig.Root.Tokens.SpeakerTokens.Select(
            token => new GatewayClient(new BotToken(token))
        );
        
        Clients = [main, ..speakerClients];
    }

    public async Task AfterStartupAsync() {
        Emojis = await Main.Rest.GetApplicationEmojisAsync(Main.Id);
    }

    public async Task<User?> GetDiscordUser(ulong id) {
        foreach (var client in Clients) {
            try {
                return await client.Rest.GetUserAsync(id);
            }
            catch {
                //ignored
            }
        }
        
        return null;
    }

    public VoiceState? GetUserVoiceState(ulong userId) {
        foreach (var client in Clients) {
            foreach (var guild in client.Cache.Guilds.Values) {
                if (guild.VoiceStates.TryGetValue(userId, out var voiceState)) {
                    return voiceState;
                }
            }
        }
        
        return null;
    }

    public GatewayClient? GetAvailableClient(ulong guildId) {
        foreach (var client in Clients) {
            if (!client.Cache.Guilds.TryGetValue(guildId, out var guild))
                return client;
            if (!guild.VoiceStates.TryGetValue(client.Cache.User?.Id ?? 0, out _))
                return client;
        }

        return null;
    }
    
    public void Shutdown() {
        var tasks = Clients.Select(
            client => client.CloseAsync()
        );

        foreach (var task in tasks) {
            task.GetAwaiter().GetResult();
        }
    }
}
