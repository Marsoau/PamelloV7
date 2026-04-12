using NetCord;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Module.Marsoau.NetCord.Extensions;

public static class PamelloUserRepositoryExtensions
{
    extension(IPamelloUserRepository users)
    {
        public async Task<IPamelloUser> GetByInteractionRequired(Interaction interaction)
            => await users.GetByInteraction(interaction) ?? throw new PamelloException("User could not be found/created");
        public async Task<IPamelloUser?> GetByInteraction(Interaction interaction)
            => await users.GetByPlatformKey(new PlatformKey("discord", interaction.User.Id.ToString()), ServerConfig.Root.AllowUserCreation);
    }
}
