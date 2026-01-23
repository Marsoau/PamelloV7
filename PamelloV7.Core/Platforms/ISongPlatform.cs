using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Core.Platforms;

public interface ISongPlatform
{
    public string Name { get; }
    
    public string ValueToKey(string value);
    public ISongInfo? GetSongInfo(string key);
    public string GetSongUrl(string key);
}
