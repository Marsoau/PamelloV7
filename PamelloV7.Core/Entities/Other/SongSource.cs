using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Downloads;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Core.Services;

namespace PamelloV7.Core.Entities.Other;

public class SongSource
{
    private readonly IServiceProvider _services;
    
    private readonly IPlatformService _platfroms;
    private readonly IDownloadService _downloads;
    private readonly IFileAccessService _files;
    
    public IPamelloSong Song { get; }

    public ISongInfo? Info { get; private set; }

    public PlatformKey PK { get; }

    public SongSource(IServiceProvider services, IPamelloSong song, PlatformKey pk) {
        _services = services;
        
        _platfroms = services.GetRequiredService<IPlatformService>();
        _downloads = services.GetRequiredService<IDownloadService>();
        _files = services.GetRequiredService<IFileAccessService>();
        
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
        
        Song.StartChanges();
        
        Song.Name = Info.Name;
        Song.CoverUrl = Info.CoverUrl;
        
        Song.RemoveAllEpisodes();
        foreach (var episodeInfo in Info.Episodes) {
            Song.AddEpisode(episodeInfo, false);
        }
        
        Song.EndChanges();
    }
    
    public bool IsDownloaded()
        => _downloads.IsDownloaded(this);
    public SongDownloader GetDownloader()
        => _downloads.GetSongDownloader(this);
    public FileInfo GetFile()
        => _files.GetSourceFile(this);
}
