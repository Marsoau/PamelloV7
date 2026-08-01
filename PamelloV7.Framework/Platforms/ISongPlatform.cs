using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Framework.Platforms;

public interface ISongPlatform
{
    public string Name { get; }
    
    public string ValueToKey(string value);
    public bool IsPlaylistValue(string value);
    public string GetSongUrl(string key);
    
    public Task<ISongInfo?> GetSongInfoAsync(string key);
    public IAsyncEnumerable<ISongInfo> GetPlaylistSongsInfoAsync(string value);
    
    public void Startup() { }
    public void Shutdown() { }
}
