using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Services;
using PamelloV7.Plugin.Base.Entites.Database;
using PamelloV7.Plugin.Base.Services;

namespace PamelloV7.Plugin.Base;

public class Base : IPamelloPlugin
{
    public string Name => "Base";
    public string Description => "Base functionality of PamelloV7";

    public void Startup(IServiceProvider services) {
        var data = services.GetRequiredService<IDataAccessService>();
        var users = data.GetCollection<DatabaseUser>("users");

        Console.WriteLine($"Users: ({users.Count()} users)");
        
        foreach (var user in users.GetAll()) {
            Console.WriteLine($"[{user.Id}] {user.Name}");
        }
    }
}
