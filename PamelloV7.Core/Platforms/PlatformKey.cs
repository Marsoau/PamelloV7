namespace PamelloV7.Core.Platforms;

public record PlatformKey(string Platform, string Key)
{
    public override string ToString() {
        return $"{Platform}_{Key}";
    }
}
