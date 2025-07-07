using System.ComponentModel.Design;

namespace PamelloV7.Core.Services.Base;

public interface IPamelloService : IDisposable
{
    public void Configure(IServiceContainer services) { }
    public void Startup(IServiceProvider services) { }

    public void Shoutdown() { }
}
