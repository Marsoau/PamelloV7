using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Platforms;

public interface ISongDownloader
{
    public IPamelloSong Song { get; }
    
    public Task Download();
}
