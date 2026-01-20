namespace PamelloV7.Core.Services.Base;

public interface IPamelloService : IDisposable
{
    public void Startup(IServiceProvider services) { }

    public void Shutdown() { }

    public new void Dispose() { }
    
    void IDisposable.Dispose() {
        Dispose();
    }
}
