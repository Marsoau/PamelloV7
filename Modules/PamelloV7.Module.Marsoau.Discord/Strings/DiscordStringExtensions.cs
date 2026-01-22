using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Discord.Services;

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

    public static async Task<DiscordString> ToDiscordString(this SongSource songSource, IServiceProvider services,
        bool withName = false) {
        var clients = services.GetRequiredService<DiscordClientService>();

        var emotes = await clients.Main.GetApplicationEmotesAsync();
        var emote = emotes.FirstOrDefault(x => x.Name == songSource.PK.Platform);

        var name = withName ? songSource.Info?.Name ?? "No Name" : songSource.PK.Key;

        return new DiscordString(
            $"{(emote is not null ? DiscordString.Emote(emote) : new DiscordString())} {DiscordString.Url(name, songSource.GetUrl())}");
    }
}
