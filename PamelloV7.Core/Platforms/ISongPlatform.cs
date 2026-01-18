using PamelloV7.Core.Model.Entities;

namespace PamelloV7.Core.Platforms;

public interface ISongPlatform
{
    public string Name { get; }
    public string IconUrl { get; }
    
    public string? ValueToKey(string value);
    public ISongInfo? GetSongInfo(string value);
    public IPamelloSong? GetSong(string value, bool createIfNotExist = false);
}
