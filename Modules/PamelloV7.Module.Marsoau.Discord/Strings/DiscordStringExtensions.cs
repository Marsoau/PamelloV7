using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Strings;

public static class DiscordStringExtensions
{
    public static string ToDiscordString(this IPamelloEntity entity)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{entity.Id}]"))} {DiscordString.Ecranate(entity.Name)}";
    }
    
    public static string ToDiscordString(this IPamelloUser user)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{user.Id}]"))} {DiscordString.User(user)}";
    }

    public static async Task<string> ToDiscordString(this SongSource songSource, IServiceProvider services, bool withName = false) {
        var clients = services.GetRequiredService<DiscordClientService>();

        var emotes = await clients.Main.GetApplicationEmotesAsync();
        var emote = emotes.FirstOrDefault(x => x.Name == songSource.PK.Platform);

        var name = withName ? songSource.Info?.Name ?? "No Name" : songSource.PK.Key;

        return $"{DiscordString.Emote(emote)} {DiscordString.Url(name, songSource.GetUrl())}";
    }
}
