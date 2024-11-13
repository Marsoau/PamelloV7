using Discord.Interactions;
using Discord.WebSocket;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model.Interactions
{
    public class PamelloSocketInteractionContext : SocketInteractionContext
    {
        public readonly PamelloUser User;

        public PamelloSocketInteractionContext(IServiceProvider services,
            SocketInteraction interaction,
            PamelloUser pamelloUser
        ) : base(
            services.GetRequiredService<DiscordClientService>().MainClient,
            interaction
        ) {
            User = pamelloUser;
        }
    }
}
