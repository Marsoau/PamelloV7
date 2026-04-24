using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Module.Marsoau.SoundCloud.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.SoundCloud.Platforms;

public class SoundCloudSongPlatform : ISongPlatform
{
    private readonly SoundCloudInfoGetter _infoGetter;
    
    public string Name => "soundcloud";

    public SoundCloudSongPlatform(IServiceProvider services) {
        _infoGetter = new SoundCloudInfoGetter(this, services);
    }
    
    public string ValueToKey(string value) {
        if (!value.StartsWith("https://")) throw new PamelloException($"Link expected as soundcloud platform value");
    
        var uri = new Uri(value);
    
        var host = uri.Host.StartsWith("www.") ? uri.Host[4..] : uri.Host;
        if (host != "soundcloud.com" && host != "m.soundcloud.com") throw new PamelloException($"Cant find a valid soundcloud link in value \"{value}\"");
    
        // Segments for "/artist/track-slug" -> ["/", "artist/", "track-slug"]
        // Segments for "/artist/track-slug/s-SECRET" -> ["/", "artist/", "track-slug/", "s-SECRET"]
        var segments = uri.Segments
            .Skip(1)
            .Select(s => s.TrimEnd('/'))
            .Where(s => s.Length > 0)
            .ToArray();
    
        if (segments.Length < 2) throw new PamelloException($"Cant find a valid soundcloud track id in value \"{value}\"");
    
        var artist = segments[0];
        var track = segments[1];
    
        if (track == "sets") throw new PamelloException($"Soundcloud playlists/albums are not supported, expected a single track in value \"{value}\"");
    
        // Private/unlisted tracks have a secret token as a third segment: /artist/track/s-XXXX
        if (segments.Length >= 3 && segments[2].StartsWith("s-")) {
            return $"{artist}/{track}/{segments[2]}";
        }
    
        return $"{artist}/{track}";
    }
    public async Task<ISongInfo?> GetSongInfoAsync(string key) {
        return await _infoGetter.GetSongInfo(key);
    }
    
    public string GetSongUrl(string key)
        => GetSoundCloudUrl(key);

    public static string GetSoundCloudUrl(string key)
        => $"https://soundcloud.com/{key}";
}
