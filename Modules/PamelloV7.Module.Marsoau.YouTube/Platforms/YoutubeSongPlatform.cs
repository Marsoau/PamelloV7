using System.Diagnostics;
using System.Web;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Module.Marsoau.YouTube.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms;

public partial class YoutubeSongPlatform : ISongPlatform
{
    private readonly YoutubeInfoGetter _infoGetter;
    
    public string Name => "youtube";

    public YoutubeSongPlatform(IServiceProvider services) {
        _infoGetter = new YoutubeInfoGetter(this, services);
    }

    public string ValueToKeyRequired(string value) {
        if (!(value.StartsWith("https://") || value.StartsWith("http://"))) {
            value = $"https://{value}";
        }
        
        var uri = new Uri(value);
        var query = HttpUtility.ParseQueryString(uri.Query);

        var id = uri.Host switch {
            "www.youtube.com" or "youtube.com" or "music.youtube.com" => query["v"],
            "youtu.be" => uri.Segments[1][..11],
            "i.ytimg.com" => uri.Segments[2][..11],
            _ => null
        };

        if (id is null || id.Length != 11 || !id.All(c => char.IsLetterOrDigit(c) || c is '-' or '_')) {
            throw new PamelloException($"Cant find a valid youtube id in value \"{value}\"");
        }

        return id;
    }

    public string ValueToPlaylistKeyRequired(string value) {
        if (!(value.StartsWith("https://") || value.StartsWith("http://"))) {
            value = $"https://{value}";
        }
        
        var uri = new Uri(value);
        var query = HttpUtility.ParseQueryString(uri.Query);

        var id = uri.Host switch {
            "www.youtube.com" or "youtube.com" or "music.youtube.com" => query["list"],
            "youtu.be" => throw new PamelloException("Cannot get playlist key from a short url"),
            "i.ytimg.com" => throw new PamelloException("Cannot get playlist key from image url"),
            _ => null
        };
        
        if (id is null || !YoutubeValidPlaylistIdRegex().IsMatch(id)) {
            throw new PamelloException($"Cant find a valid youtube playlist id in value \"{value}\"");
        }
        
        return id;
    }


    public async Task<ISongInfo?> GetSongInfoAsync(string key) {
        return await _infoGetter.GetSongInfoAsync(key);
    }

    public async IAsyncEnumerable<ISongInfo> GetPlaylistSongsInfoAsync(string playlistKey) {
        await foreach (var info in _infoGetter.GetPlaylistSongsInfoAsync(playlistKey)) {
            yield return info;
        }
    }

    public string GetSongUrl(string key)
        => GetYoutubeSongUrl(key);
    public string GetPlaylistUrl(string key)
        => GetYoutubePlaylistUrl(key);

    public static string GetYoutubeSongUrl(string key)
        => $"https://www.youtube.com/watch?v={key}";
    public static string GetYoutubePlaylistUrl(string key)
        => $"https://www.youtube.com/playlist?list={key}";
    
    [GeneratedRegex("^(PL([0-9A-Fa-f]{16}|[A-Za-z0-9_-]{32})|UU(LV|MO|SH)?[A-Za-z0-9_-]{22}|SP([0-9A-Fa-f]{16}|[A-Za-z0-9_-]{32})|OLAK5uy_[klmn][A-Za-z0-9_-]{32})$")]
    private static partial Regex YoutubeValidPlaylistIdRegex();
}
