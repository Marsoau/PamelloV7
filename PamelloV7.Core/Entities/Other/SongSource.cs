using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Core.Services;

namespace PamelloV7.Core.Entities.Other;

public class SongSource
{
    private readonly IPlatformService _platfroms;
    
    public IPamelloSong Song { get; }
    
    private ISongInfo? _info;
    public ISongInfo? Info {
        get {
            if (_info is not null) return _info;
            return UpdateInfo();
        }
    }
    
    public PlatformKey PK { get; }

    public SongSource(IServiceProvider services, IPamelloSong song, PlatformKey pk) {
        _platfroms = services.GetRequiredService<IPlatformService>();
        
        Song = song;
        PK = pk;
    }

    public ISongInfo? UpdateInfo() {
        var platform = _platfroms.GetSongPlatform(PK.Platform);
        if (platform is null) return _info = null;
        
        var info = platform.GetSongInfo(PK.Key);
        return _info = info;
    }

    public void SetFromInfo() {
        if (Info is null) return;
        
        //lock song update events
        Song.StartChangesAsync().Wait();
        
        Song.Name = Info.Name;
        Song.CoverUrl = Info.CoverUrl;
        
        Song.RemoveAllEpisodes();
        foreach (var episodeInfo in Info.Episodes) {
            Song.AddEpisode(episodeInfo, false);
        }
        
        //release song update events
        Song.EndChanges();
    }
}
