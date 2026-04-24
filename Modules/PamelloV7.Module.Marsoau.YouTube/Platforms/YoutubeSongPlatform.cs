using System.Diagnostics;
using System.Web;
using System.Net.Http;
using System.Text.Json;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Module.Marsoau.YouTube.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms;

public class YoutubeSongPlatform : ISongPlatform
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly YoutubeInfoGetter _infoGetter;
    
    public string Name => "youtube";

    public YoutubeSongPlatform(IServiceProvider services) {
        _httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
        
        _infoGetter = new YoutubeInfoGetter(this, services);
    }

    public string ValueToKey(string value) {
        var id = value;

        if (id.StartsWith("https://")) {
            Uri uri;

            uri = new Uri(value);
            var query = HttpUtility.ParseQueryString(uri.Query);

            id = uri.Host switch {
                "www.youtube.com" => query["v"],
                "youtu.be" => uri.Segments[1][..11],
                "i.ytimg.com" => uri.Segments[2][..11],
                _ => null
            };
        }

        if (id is null || id.Length != 11 || !id.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')) {
            throw new PamelloException($"Cant find a valid youtube id in value \"{value}\"");
        }

        return id;
    }
    
    public async Task<ISongInfo?> GetSongInfoAsync(string value) {
        string youtubeId;
        
        try {
            youtubeId = ValueToKey(value);
        }
        catch {
            return null;
        }

        return await _infoGetter.GetSongInfo(youtubeId);
    }

    public string GetSongUrl(string key) {
        return $"https://www.youtube.com/watch?v={key}";
    }
}
