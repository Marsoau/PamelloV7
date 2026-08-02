using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Framework.Platforms;

public interface ISongPlatform
{
    public string Name { get; }
    
    public string ValueToKeyRequired(string value);
    public string ValueToPlaylistKeyRequired(string value);
    
    public (string? songId, string? playlistId) ValueToKeys(string value) {
        string? songId = null;
        
        try {
            songId = ValueToKeyRequired(value);
        } catch {
            //ignored
        }

        string? playlistId = null;
        
        try {
            playlistId = ValueToPlaylistKeyRequired(value);
        } catch {
            //ignored
        }
        
        return (songId, playlistId);
    }
    
    public Task<ISongInfo?> GetSongInfoAsync(string songKey);
    public IAsyncEnumerable<ISongInfo> GetPlaylistSongsInfoAsync(string playlistKey);
    
    public string GetSongUrl(string songKey);
    public string GetPlaylistUrl(string playlistKey);
    
    public void Startup() { }
    public void Shutdown() { }
}
