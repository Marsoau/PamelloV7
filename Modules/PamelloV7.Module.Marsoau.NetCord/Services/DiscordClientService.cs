using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Logger;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class DiscordClientService : IPamelloService
{
    public GatewayClient[] Clients = [];
    public GatewayClient Main => Clients.FirstOrDefault() ?? throw new PamelloException("Main client is not registered yet");

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
    
    public void Shutdown() {
        foreach (var client in Clients) {
            client.Dispose();
        }
    }
}
