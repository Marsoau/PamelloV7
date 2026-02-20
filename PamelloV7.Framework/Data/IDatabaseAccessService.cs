using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Data;

public interface IDatabaseAccessService : IPamelloService
{
    public IDatabaseCollection<TType> GetCollection<TType>(string name);
}
