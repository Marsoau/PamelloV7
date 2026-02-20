namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SongDownloaderAttribute : Attribute
{
    public string Name { get; }
    
    public SongDownloaderAttribute(string name) {
        Name = name;
    }
}
