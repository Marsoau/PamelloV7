namespace PamelloV7.Framework.Services.Base;

public interface IPamelloService : IDisposable
{
    public void Startup(IServiceProvider services) { }

    public void Shutdown() { }

    public new void Dispose() { }
    
    void IDisposable.Dispose() {
        Dispose();
    }
}
