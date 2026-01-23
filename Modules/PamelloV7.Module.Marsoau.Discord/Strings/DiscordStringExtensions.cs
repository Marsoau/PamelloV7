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
    
    public static string ToDiscordString(this IPamelloPlaylist playlist)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{playlist.Id}]"))} {DiscordString.Ecranate(playlist.Name)} {DiscordString.None(DiscordString.Code($"({playlist.Songs.Count})"))}";
    }
    
    public static string ToDiscordString(this IPamelloUser user, bool mention = true)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{user.Id}]"))} {(mention ? DiscordString.User(user) : user.Name)}";
    }

    public static async Task<string> ToDiscordString(this SongSource songSource, IServiceProvider services, bool withName = false) {
        var clients = services.GetRequiredService<DiscordClientService>();

        var emote = await clients.GetEmote(songSource.PK.Platform);

        var name = withName ? songSource.Info?.Name ?? "No Name" : songSource.PK.Key;

        return $"{DiscordString.Emote(emote)} {DiscordString.Url(name, songSource.GetUrl())}";
    }
}
