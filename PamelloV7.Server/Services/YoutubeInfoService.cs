using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Model.Youtube;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using System.Text.Json;

namespace PamelloV7.Server.Services
{
    public class YoutubeInfoService
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public readonly IServiceProvider _services;

        public YoutubeInfoService(IHttpClientFactory httpClientFactory, IServiceProvider services)
        {
            _httpClientFactory = httpClientFactory;
            _services = services;
        }

        public string GetVideoIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            string? youtubeId = null;
            if (uri.Host == "youtube.com") {
                youtubeId = query["v"];
            }
            else if (uri.Host == "youtu.be") {
                youtubeId = uri.Segments[1].Substring(0, 11);
            }

            if (youtubeId is null || youtubeId.Length != 11) {
                throw new PamelloException($"cant find youtube id in url \"{url}\"");
            }

            return youtubeId;
        }

        public async Task<YoutubeVideoInfo?> GetVideoInfoAsync(string youtubeId)
        {
            using var client = _httpClientFactory.CreateClient();
            var responce = await client.GetAsync($"https://www.youtube.com/watch?v={youtubeId}");
            if (!responce.IsSuccessStatusCode) return null;

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var stringContent = responce.Content.ReadAsStream();
            var html = await context.OpenAsync(response => response.Content(stringContent));

            var youtubeVideoInfo = new YoutubeVideoInfo();

            bool isNameFound = false;

            var metaElements = html.QuerySelectorAll("meta");
            foreach (var metaElement in metaElements)
            {
                if (metaElement.GetAttribute("name") == "title")
                {
                    var name = metaElement.GetAttribute("content");
                    if (name is null || name.Length == 0) return null;

                    youtubeVideoInfo.Name = name;
                    isNameFound = true;
                    break;
                }
            }

            if (!isNameFound) return null;

            var span = html.QuerySelectorAll("span").First(
                s => s.GetAttribute("itemprop") == "author"
            );
            var link = span.QuerySelectorAll("link").First(l => l.GetAttribute("itemprop") == "name");

            youtubeVideoInfo.Channel = link.GetAttribute("content") ?? "";

            var json = GetVideoJson(html);

            youtubeVideoInfo.Episodes = await GetVideoEpisodes(json);

            return youtubeVideoInfo;
        }

        private JsonDocument GetVideoJson(IDocument videoHtml)
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

            File.WriteAllText(@"D:\json\v.json", jsonStr);

            return JsonDocument.Parse(jsonStr ?? "{}");
        }

        private async Task<List<YoutubeEpisodeInfo>> GetVideoEpisodes(JsonDocument videoJson)
        {
            var episodes = new List<YoutubeEpisodeInfo>();

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
                episodes.Add(new YoutubeEpisodeInfo()
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

        /*
        public async Task<YoutubeSearchResult> Search(int pageSize, string? query)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = PamelloConfig.YoutubeToken,
            });

            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.MaxResults = pageSize;
            searchRequest.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;

            var searchResponse = await searchRequest.ExecuteAsync();
            var searchResult = new YoutubeSearchResult();

            var songs = _services.GetRequiredService<PamelloSongRepository>();
            PamelloSong? song;

            foreach (SearchResult result in searchResponse.Items)
            {
                if (result?.Id?.VideoId is not null)
                {
                    song = songs.GetByYoutubeId(result.Id.VideoId);

                    if (song is not null)
                    {
                        searchResult.PamelloSongs.Add(song);
                    }
                    else
                    {
                        searchResult.YoutubeVideos.Add(new YoutubeSearchVideoInfo()
                        {
                            Id = result.Id.VideoId,
                            Name = result.Snippet.Title,
                            Author = result.Snippet.ChannelTitle,
                            ThumbnailUrl = result.Snippet.Thumbnails.Default__?.Url ?? "",
                        });
                    }
                }
            }

            return searchResult;
        }
        */
    }
}
