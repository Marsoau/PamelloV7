namespace PamelloV7.Core.Services.Base;

public interface IPamelloService : IDisposable
{
    public void Startup();
    public void Shoutdown();
}
