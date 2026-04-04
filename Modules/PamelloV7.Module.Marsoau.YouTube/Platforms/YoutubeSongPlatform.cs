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
    
    public string Name => "youtube";

    public YoutubeSongPlatform(IServiceProvider services) {
        _httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    }

    public void Startup() {
        /*
        StaticLogger.Log("Starting youtube token server");
        Console.ReadLine();
        if (_tokenServerProcess is not null) return;
        
        _tokenServerProcess = new Process();
        _tokenServerProcess.StartInfo = new ProcessStartInfo() {
            FileName = $"{AppContext.BaseDirectory}utils/bgutil-pot",
            Arguments = "server",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        
        _tokenServerProcess.Start();
        */
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
        
        return await Task.Run(() => GetVideoInfoAsync(youtubeId));
    }

    public string GetSongUrl(string key) {
        return $"https://www.youtube.com/watch?v={key}";
    }

    public async Task<ISongInfo?> GetVideoInfoAsync(string youtubeId)
    {
        using var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://www.youtube.com/watch?v={youtubeId}");
        if (!response.IsSuccessStatusCode) return null;

        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var stringContent = response.Content.ReadAsStream();
        var html = await context.OpenAsync(res => res.Content(stringContent));

        var youtubeVideoInfo = new YoutubeVideoInfo(this, youtubeId);

        JsonDocument json;
        try 
        {
            json = GetVideoBigJson(html);
        }
        catch 
        {
            return null;
        }

        try
        {
            var videoDetails = json.RootElement
                .GetProperty("playerOverlays")
                .GetProperty("playerOverlayRenderer")
                .GetProperty("videoDetails")
                .GetProperty("playerOverlayVideoDetailsRenderer");

            youtubeVideoInfo.Title = videoDetails
                .GetProperty("title")
                .GetProperty("simpleText")
                .GetString() ?? "";

            youtubeVideoInfo.Channel = videoDetails
                .GetProperty("subtitle")
                .GetProperty("runs")[0]
                .GetProperty("text")
                .GetString() ?? "";

            if (string.IsNullOrEmpty(youtubeVideoInfo.Title)) return null;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }

        var ogImage = html.QuerySelector("meta[property='og:image']")?.GetAttribute("content");
        youtubeVideoInfo.CoverUrl = ogImage ?? $"https://i.ytimg.com/vi/{youtubeId}/hqdefault.jpg";

        youtubeVideoInfo.Episodes = await GetVideoEpisodes(json, youtubeVideoInfo);

        return youtubeVideoInfo;
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
        
        File.WriteAllText("/home/marsoau/YOUTUBE_JSON.json", jsonStr);

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
