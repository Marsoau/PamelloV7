using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Data;

public interface IDatabaseAccessService : IPamelloService
{
    public IDatabaseCollection<TType> GetCollection<TType>(string name);
}
