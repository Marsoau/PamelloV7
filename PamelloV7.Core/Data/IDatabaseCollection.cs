using System.Linq.Expressions;

namespace PamelloV7.Core.Data;

public interface IDatabaseCollection<TDatabaseEntity>
{
    public TDatabaseEntity Get(object key);
    public IEnumerable<TDatabaseEntity> GetAll();
    
    public int Count();
    
    public void Add(TDatabaseEntity entity);
    public void Save(TDatabaseEntity entity);
    public void Delete(object key);
    public void DeleteMany(Expression<Func<TDatabaseEntity, bool>> predicate);
}
