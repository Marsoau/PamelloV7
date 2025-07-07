using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Repositories.Base;

public interface IPamelloRepository<TEntity> : IDisposable, IPamelloService
    where TEntity : IPamelloEntity
{
    public event Action? BeforeLoading;
    public event Action<int, int>? OnLoadingProgress;
    public event Action? OnLoaded;

    public event Action? BeforeInit;
    public event Action<int, int>? OnInitProgress;
    public event Action? OnInit;

    public void InitServices();
    public Task LoadAllAsync();
    public Task InitAllAsync();
    
    public TEntity? Get(int id);
    public TEntity GetRequired(int id);

    public Task<TEntity?> GetByValue(string value, IPamelloUser? scopeUser);
    public Task<TEntity> GetByValueRequired(string value, IPamelloUser? scopeUser);

    public Task<IEnumerable<TEntity>> SearchAsync(string query, IPamelloUser? scopeUser = null);

    public void Delete(TEntity entity);
}