using System.Linq.Expressions;
using LiteDB;
using PamelloV7.Core.Data;
using PamelloV7.Core.Data.Entities.Base;

namespace PamelloV7.Server.Database;

public class DatabaseCollection<TDatabaseEntity> : IDatabaseCollection<TDatabaseEntity>
{
    private ILiteCollection<TDatabaseEntity> _collection;
    
    public DatabaseCollection(ILiteCollection<TDatabaseEntity> collection) {
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

    public void Delete(object key) {
        _collection.Delete((int)key);
    }
    public void DeleteMany(Expression<Func<TDatabaseEntity, bool>> predicate) {
        _collection.DeleteMany(predicate);
    }

    public void Drop() {
        _collection.DeleteAll();
    }
}
