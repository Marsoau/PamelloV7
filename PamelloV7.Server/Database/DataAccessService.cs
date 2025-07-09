using LiteDB;
using PamelloV7.Core.Data;
using PamelloV7.Server.Config;

namespace PamelloV7.Server.Database;

public class DataAccessService : IDataAccessService
{
    private LiteDatabase _db;
    
    public void Startup(IServiceProvider services) {
        _db = new LiteDatabase($"{PamelloServerConfig.Root.DataPath}/ldb.db");
    }

    public IDataCollection<TType> GetCollection<TType>(string name) {
        var collection = _db.GetCollection<TType>(name);

        return new DataCollection<TType>(collection);
    }
}
