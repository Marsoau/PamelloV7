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
        public async Task<IPamelloUser> GetByInteractionRequired(Interaction interaction, bool allowCreation = false)
            => await users.GetByInteraction(interaction, allowCreation) ?? throw new PamelloException("User could not be found/created");
        public async Task<IPamelloUser?> GetByInteraction(Interaction interaction, bool allowCreation = false)
            => await users.GetByDiscordId(interaction.User.Id, allowCreation);
        
        public async Task<IPamelloUser?> GetByDiscordId(ulong discordId, bool allowCreation = false)
            => await users.GetByPlatformKey(new PlatformKey("discord", discordId.ToString()), allowCreation);
    }
}
