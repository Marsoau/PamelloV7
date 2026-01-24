using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Module.Marsoau.Osu.Platforms.Infos;
using PamelloV7.Module.Marsoau.Osu.Services;

namespace PamelloV7.Module.Marsoau.Osu.Platforms;

public class OsuSongPlatform : ISongPlatform
{
    private readonly OsuClientService _client;
    
    public string Name => "osu";
    
    public OsuSongPlatform(IServiceProvider services) {
        _client = services.GetRequiredService<OsuClientService>();
    }
    
    public string ValueToKey(string value) {
        if (!value.StartsWith("https://")) throw new PamelloException($"Link expected as osu platform value");
        
        var uri = new Uri(value);

        if (uri.Host != "osu.ppy.sh" || uri.Segments[1] != "beatmapsets/") throw new PamelloException($"Cant find a valid osu link in value \"{value}\"");
        
        var id = uri.Segments[2];
        if (id is null) throw new PamelloException($"Cant find a valid osu id in value \"{value}\"");

        return id;
    }

    public async Task<ISongInfo?> GetSongInfoAsync(string key) {
        if (!int.TryParse(key, out var id)) return null;
        
        var set = (await _client.Client.GetBeatmapSetAsync(id)).Value;
        if (set is null) return null;
        
        return new OsuSongInfo(this, set);
    }

    public string GetSongUrl(string key) {
        return $"https://osu.ppy.sh/beatmapsets/{key}";
    }
}
