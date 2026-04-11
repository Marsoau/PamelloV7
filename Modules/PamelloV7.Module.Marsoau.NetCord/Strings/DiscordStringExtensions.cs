using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Strings;

public static class DiscordStringExtensions
{
    public static string ToDiscordString(this IPamelloEntity entity)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{entity.Id}]"))} {DiscordString.Ecranate(entity.Name)}";
    }
    public static string ToDiscordString(this IPamelloInternetSpeaker internetSpeaker)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{internetSpeaker.Id}]"))} {DiscordString.Url(DiscordString.Ecranate(internetSpeaker.Name), internetSpeaker.GetUrl())}";
    }
    
    public static string ToDiscordString(this IPamelloPlaylist playlist)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{playlist.Id}]"))} {DiscordString.Ecranate(playlist.Name)} {DiscordString.None(DiscordString.Code($"({playlist.Songs.Count})"))}";
    }
    
    public static string ToDiscordString(this IPamelloEpisode episode, bool shortTimeString = true, bool withSongId = true)
    {
        return $"{DiscordString.Code(
            shortTimeString
                ? episode.Start.ToShortString()
                : episode.Start.ToString()
        )} {(
            episode.AutoSkip
                ? DiscordString.Crossed(DiscordString.Ecranate(episode.Name))
                : DiscordString.Ecranate(episode.Name)
        )}{(
            withSongId
                ? $" {DiscordString.Bold(DiscordString.Code($"[{episode.Song.Id}]"))}"
                : ""
        )}";
    }
    
    public static string ToDiscordString(this IPamelloUser user, bool mention = true)
    {
        return $"{DiscordString.Bold(DiscordString.Code($"[{user.Id}]"))} {(mention ? DiscordString.User(user) : user.Name)}";
    }
}
