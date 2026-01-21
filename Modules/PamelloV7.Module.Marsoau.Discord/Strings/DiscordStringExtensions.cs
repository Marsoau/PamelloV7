using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Module.Marsoau.Discord.Strings;

public static class DiscordStringExtensions
{
    public static DiscordString ToDiscordString(this IPamelloEntity entity)
    {
        return new DiscordString($"{DiscordString.Code($"[{entity.Id}]")} {DiscordString.Ecranate(entity.Name)}");
    }
    
    public static DiscordString ToDiscordString(this IPamelloUser user)
    {
        return new DiscordString($"{DiscordString.Code($"[{user.Id}]")} {new DiscordString(user)}");
    }
}
