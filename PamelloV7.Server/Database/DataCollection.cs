using LiteDB;
using PamelloV7.Core.Data;

namespace PamelloV7.Server.Database;

public class DataCollection<TDatabaseEntity> : IDataCollection<TDatabaseEntity>
{
    private ILiteCollection<TDatabaseEntity> _collection;
    
    public DataCollection(ILiteCollection<TDatabaseEntity> collection) {
        _collection = collection;
    }

    public TDatabaseEntity Get(object key) {
        return _collection.FindById((int)key);
    }

    public IEnumerable<TDatabaseEntity> GetAll() {
        return _collection.FindAll();
    }

    public int Count() {
        return _collection.Count();
    }

    public void Add(TDatabaseEntity entity) {
        _collection.Insert(entity);
    }

    public void Save(TDatabaseEntity entity) {
        _collection.Update(entity);
    }
}
