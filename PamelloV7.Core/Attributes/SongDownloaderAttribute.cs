namespace PamelloV7.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SongDownloaderAttribute : Attribute
{
    public string Name { get; }
    
    public SongDownloaderAttribute(string name) {
        Name = name;
    }
}
