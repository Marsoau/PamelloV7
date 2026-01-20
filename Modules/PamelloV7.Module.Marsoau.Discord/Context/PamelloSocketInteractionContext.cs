using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Context
{
    public class PamelloSocketInteractionContext : SocketInteractionContext
    {
        public SocketUser DiscordUser => base.User;
        public new readonly IPamelloUser User;

        public PamelloSocketInteractionContext(
            IServiceProvider services,
            SocketInteraction interaction,
            IPamelloUser pamelloUser
        ) : base(
            services.GetRequiredService<DiscordClientService>().Main,
            interaction
        ) {
            User = pamelloUser;
        }
    }
}
