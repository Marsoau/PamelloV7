namespace PamelloV7.Framework.Platforms;

public record PlatformKey(string Platform, string Key)
{
    public override string ToString() {
        return $"{Platform}_{Key}";
    }

    public static PlatformKey? FromString(string str) {
        var position = str.IndexOf('_');
        if (position == -1) return null;
        
        return new PlatformKey(str[..position], str[(position + 1)..]);
    }
}
