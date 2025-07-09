namespace PamelloV7.Core.Data;

public interface IDataCollection<TDatabaseEntity>
{
    public TDatabaseEntity Get(object key);
    public IEnumerable<TDatabaseEntity> GetAll();
    
    public int Count();
    
    public void Add(TDatabaseEntity entity);
    public void Save(TDatabaseEntity entity);
}
