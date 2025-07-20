using LiteDB;
using PamelloV7.Core.Data;
using PamelloV7.Server.Config;

namespace PamelloV7.Server.Database;

public class DatabaseAccessService : IDatabaseAccessService
{
    private readonly LiteDatabase _db;

    public DatabaseAccessService() {
        _db = new LiteDatabase($"{PamelloServerConfig.Root.DataPath}/lite.db");
    }

    public IDatabaseCollection<TType> GetCollection<TType>(string name) {
        var collection = _db.GetCollection<TType>(name);

        return new DatabaseCollection<TType>(collection);
    }
}
