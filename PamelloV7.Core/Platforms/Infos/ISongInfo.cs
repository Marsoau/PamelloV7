namespace PamelloV7.Core.Platforms;

public interface ISongInfo
{
    public ISongPlatform Platform { get; }
    
    public string Key { get; }
    public string Name { get; }
    public string CoverUrl { get; }
    
    public List<IEpisodeInfo> Episodes { get; }
}
