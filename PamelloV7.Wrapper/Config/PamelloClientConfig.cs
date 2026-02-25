namespace PamelloV7.Wrapper.Config;

public record PamelloClientConfig()
{
    public string? BaseUrl { get; internal set; }
    public Guid? Token { get; internal set; }
}
