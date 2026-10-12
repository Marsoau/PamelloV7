using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Framework.Platforms;

public interface ISongPlatform
{
    public string Name { get; }
    
    public string ValueToKeyRequired(string value);
    public IAsyncEnumerable<string> ValueToManyKeysAsync(string value);
    
    public Task<ISongInfo?> GetSongInfoAsync(string songKey);
    
    public string GetSongUrl(string songKey);
    
    public void Startup() { }
    public void Shutdown() { }
}
