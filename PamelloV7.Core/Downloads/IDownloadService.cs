using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Downloads;

public interface IDownloadService : IPamelloService
{
    public bool DoesSongDownloaderExist(string platform);
    public SongDownloader GetSongDownloader(SongSource source);
    public bool IsDownloading(SongSource source);
    public bool IsDownloaded(SongSource source);
    public void RemoveDownloader(SongDownloader downloader);
}
