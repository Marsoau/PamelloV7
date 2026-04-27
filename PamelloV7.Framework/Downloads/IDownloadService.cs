using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Downloads;

public interface IDownloadService : IPamelloService
{
    public bool DoesSongDownloaderExist(string platform);
    public SongDownloader GetSongDownloaderRequired(SongSource source);
    public SongDownloader? GetSongDownloader(SongSource source);
    public bool IsDownloading(SongSource? source);
    public bool IsDownloaded(SongSource? source);
    public void RemoveDownloader(SongDownloader downloader);
}
