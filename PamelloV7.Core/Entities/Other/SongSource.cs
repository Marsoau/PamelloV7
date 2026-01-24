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

    public ISongInfo? Info { get; private set; }

    public PlatformKey PK { get; }

    public SongSource(IServiceProvider services, IPamelloSong song, PlatformKey pk) {
        _platfroms = services.GetRequiredService<IPlatformService>();
        
        Song = song;
        PK = pk;
    }

    public async Task<ISongInfo?> UpdateInfo() {
        var platform = _platfroms.GetSongPlatform(PK.Platform);
        if (platform is null) return Info = null;
        
        var info = await platform.GetSongInfoAsync(PK.Key);
        return Info = info;
    }

    public string GetUrl() {
        var platform = _platfroms.GetSongPlatform(PK.Platform);
        if (platform is null) return "";
        
        return platform.GetSongUrl(PK.Key);
    }

    public void SetInfoToSong() {
        if (Info is null) return;
        
        //lock song update events
        Song.StartChanges();
        
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
