using System.Web;
using System.Net.Http;
using System.Text.Json;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Module.Marsoau.YouTube.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.YouTube.Platforms;

public class YoutubeSongPlatform : ISongPlatform
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public string Name => "youtube";

    public YoutubeSongPlatform(IServiceProvider services) {
        _httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
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
    
    public ISongInfo? GetSongInfo(string value) {
        string youtubeId;
        
        try {
            youtubeId = ValueToKey(value);
        }
        catch {
            return null;
        }
        
        return GetVideoInfoAsync(youtubeId).Result;
    }

    public string GetSongUrl(string key) {
        return $"https://www.youtube.com/watch?v={key}";
    }

    public async Task<ISongInfo?> GetVideoInfoAsync(string youtubeId)
    {
        using var client = _httpClientFactory.CreateClient();
        var responce = await client.GetAsync($"https://www.youtube.com/watch?v={youtubeId}");
        if (!responce.IsSuccessStatusCode) return null;

        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var stringContent = responce.Content.ReadAsStream();
        var html = await context.OpenAsync(response => response.Content(stringContent));

        var youtubeVideoInfo = new YoutubeVideoInfo(this, youtubeId);

        bool isTitleFound = false;

        var metaElements = html.QuerySelectorAll("meta");
        foreach (var metaElement in metaElements)
        {
            if (metaElement.GetAttribute("name") == "title")
            {
                var title = metaElement.GetAttribute("content");
                if (title is null || title.Length == 0) return null;

                youtubeVideoInfo.Title = title;
                isTitleFound = true;
                break;
            }
        }

        if (!isTitleFound) return null;

        var span = html.QuerySelectorAll("span").First(
            s => s.GetAttribute("itemprop") == "author"
        );
        var link = span.QuerySelectorAll("link").First(l => l.GetAttribute("itemprop") == "name");

        youtubeVideoInfo.Channel = link.GetAttribute("content") ?? "";
            
        youtubeVideoInfo.CoverUrl = GetVideoCoverUrl(html) ?? $"https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg";

        var json = GetVideoBigJson(html);

        youtubeVideoInfo.Episodes = await GetVideoEpisodes(json, youtubeVideoInfo);

        return youtubeVideoInfo;
    }
    
    private string? GetVideoCoverUrl(IDocument videoHtml) {
        string? jsonStr = null;
        IHtmlCollection<IElement> scriptElements = videoHtml.QuerySelectorAll("script");
        foreach (IElement scriptElement in scriptElements) {
            if (scriptElement.InnerHtml.StartsWith("{\"@context\"")) {
                jsonStr = scriptElement.InnerHtml;
                break;
            }
        }

        if (jsonStr is null) return null;
            
        var smallJson = JsonDocument.Parse(jsonStr ?? "{}");

        try {
            return smallJson.RootElement.GetProperty("thumbnailUrl").ToString();
        }
        catch {
            return null;
        }
    }
    private JsonDocument GetVideoBigJson(IDocument videoHtml)
    {
        string? jsonStr = null;
        IHtmlCollection<IElement> scriptElements = videoHtml.QuerySelectorAll("script");
        foreach (IElement scriptElement in scriptElements)
        {
            if (scriptElement.InnerHtml.StartsWith("var ytInitialData"))
            {
                jsonStr = scriptElement.InnerHtml.Substring(20, scriptElement.InnerHtml.Length - 21);
                break;
            }
        }

        if (jsonStr is null)
        {
            throw new PamelloException("Couldnt find requires json object in html");
        }

        return JsonDocument.Parse(jsonStr ?? "{}");
    }

    private async Task<List<IEpisodeInfo>> GetVideoEpisodes(JsonDocument videoJson, ISongInfo songInfo)
    {
        var episodes = new List<IEpisodeInfo>();

        JsonElement chapterElements;
        try
        {
            chapterElements = videoJson.RootElement.GetProperty("playerOverlays")
                .GetProperty("playerOverlayRenderer")
                .GetProperty("decoratedPlayerBarRenderer")
                .GetProperty("decoratedPlayerBarRenderer")
                .GetProperty("playerBar")
                .GetProperty("multiMarkersPlayerBarRenderer")
                .GetProperty("markersMap")
                [0]
                .GetProperty("value")
                .GetProperty("chapters");
        }
        catch
        {
            return episodes;
        }

        for (int i = 0; i < chapterElements.GetArrayLength(); i++)
        {
            episodes.Add(new YoutubeEpisodeInfo(songInfo)
            {
                Name = chapterElements[i]
                    .GetProperty("chapterRenderer")
                    .GetProperty("title")
                    .GetProperty("simpleText").ToString(),

                Start = int.Parse(
                    chapterElements[i]
                        .GetProperty("chapterRenderer")
                        .GetProperty("timeRangeStartMillis").ToString()
                ) / 1000
            });
        }

        return episodes;
    }
}
