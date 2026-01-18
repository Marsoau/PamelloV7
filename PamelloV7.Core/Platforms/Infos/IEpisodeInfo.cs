namespace PamelloV7.Core.Platforms;

public interface IEpisodeInfo
{
    public ISongInfo SongInfo { get; }
    
    public string Name { get; }
    public int Start { get; }
}
