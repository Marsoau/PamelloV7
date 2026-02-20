using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Platforms;

public interface ISongDownloader
{
    public IPamelloSong Song { get; }
    
    public Task Download();
}
