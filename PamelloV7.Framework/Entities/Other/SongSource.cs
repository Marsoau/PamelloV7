using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Entities.Other;

[SafeEntity<IPamelloSong>("Song")]
public partial class SongSource
{
    private readonly IServiceProvider _services;
    
    private readonly IPlatformService _platfroms;
    private readonly IDownloadService _downloads;
    private readonly IFileAccessService _files;

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

    public void ResetSongInfo(IPamelloUser scopeUser) {
        if (Info is null) return;

        var song = _safeSong.RequiredEntity;
        
        song.StartChanges();
        
        song.SetName(Info.Name, scopeUser);
        song.SetCoverUrl(Info.CoverUrl, scopeUser);
        
        song.RemoveAllEpisodes(scopeUser);
        foreach (var episodeInfo in Info.Episodes) {
            song.AddEpisode(episodeInfo, false, scopeUser);
        }
        
        song.EndChanges();
    }
    
    public bool IsDownloaded()
        => _downloads.IsDownloaded(this);
    public SongDownloader GetDownloader()
        => _downloads.GetSongDownloader(this);
    public FileInfo GetFile()
        => _files.GetSourceFile(this);
}
