using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Framework.Platforms;

public interface ISongPlatform
{
    public string Name { get; }
    
    public string ValueToKey(string value);
    public Task<ISongInfo?> GetSongInfoAsync(string key);
    public string GetSongUrl(string key);
    
    public void Startup() { }
    public void Shutdown() { }
}
