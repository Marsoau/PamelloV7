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
    
    public string ValueToKeyRequired(string value) {
        var segments = GetSoundCloudSegments(value);
    
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

    public string ValueToPlaylistKeyRequired(string value) {
        var segments = GetSoundCloudSegments(value);

        if (segments.Length < 3 || segments[1] != "sets") throw new PamelloException($"Cant find a valid soundcloud playlist id in value \"{value}\"");

        var artist = segments[0];
        var set = segments[2];

        if (segments.Length >= 4 && segments[3].StartsWith("s-")) {
            return $"{artist}/sets/{set}/{segments[3]}";
        }

        return $"{artist}/sets/{set}";
    }

    public async Task<ISongInfo?> GetSongInfoAsync(string key) {
        return await _infoGetter.GetSongInfo(key);
    }

    public async IAsyncEnumerable<string> ValueToManyKeysAsync(string value) {
        string playlistKey;
        
        try {
            playlistKey = ValueToPlaylistKeyRequired(value);
        }
        catch { yield break; }
        
        await foreach (var trackUrl in _infoGetter.GetPlaylistSongsKeysAsync(playlistKey)) {
            string key;

            try {
                key = ValueToKeyRequired(trackUrl);
            }
            catch { continue; }

            yield return key;
        }
    }

    public string GetSongUrl(string key)
        => GetSoundCloudUrl(key);
    public string GetPlaylistUrl(string playlistKey)
        => GetSoundCloudPlaylistUrl(playlistKey);

    public static string GetSoundCloudUrl(string key)
        => $"https://soundcloud.com/{key}";
    public static string GetSoundCloudPlaylistUrl(string key)
        => $"https://soundcloud.com/{key}";

    private static string[] GetSoundCloudSegments(string value) {
        if (!value.StartsWith("https://")) throw new PamelloException($"Link expected as soundcloud platform value");
    
        var uri = new Uri(value);
    
        var host = uri.Host.StartsWith("www.") ? uri.Host[4..] : uri.Host;
        if (host != "soundcloud.com" && host != "m.soundcloud.com") throw new PamelloException($"Cant find a valid soundcloud link in value \"{value}\"");

        return uri.Segments
            .Skip(1)
            .Select(s => s.TrimEnd('/'))
            .Where(s => s.Length > 0)
            .ToArray();
    }
}