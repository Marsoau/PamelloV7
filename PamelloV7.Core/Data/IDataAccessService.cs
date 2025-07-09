using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Data;

public interface IDataAccessService : IPamelloService
{
    public IDataCollection<TType> GetCollection<TType>(string name);
}
