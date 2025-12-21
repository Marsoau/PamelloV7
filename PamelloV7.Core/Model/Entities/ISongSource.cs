namespace PamelloV7.Core.Model.Entities;

public interface ISongSource
{
    public IPamelloSong Song { get; }
    
    public string Service { get; }
    public string Id { get; }
    
    public string? InternetUrl { get; }
    
    public string Title { get; }
    public string CoverUrl { get; }
    
    //<filename> part of "[song id]-<filename>.opus" full file name
    public string FileName { get; }
    
    public bool IsDownloaded { get; set; }
}
