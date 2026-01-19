using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Repositories.Base;

public interface IPamelloDatabaseRepository
{
    public string CollectionName { get; }
    
    public event Action? OnLoadingStart;
    public event Action<int, int>? OnLoadingProgress;
    public event Action? OnLoadingEnd;

    public event Action? OnInitStart;
    public event Action<int, int>? OnInitProgress;
    public event Action? OnInitEnd;
    
    public Task LoadAllAsync();
    public Task InitAllAsync();
}
public interface IPamelloDatabaseRepository<TPamelloEntity> : IPamelloDatabaseRepository, IPamelloRepository<TPamelloEntity>
    where TPamelloEntity : IPamelloDatabaseEntity
{ }
