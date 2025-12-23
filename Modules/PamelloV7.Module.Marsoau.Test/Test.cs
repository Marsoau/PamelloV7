using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Core.Extensions;

namespace PamelloV7.Module.Marsoau.Test;

public class Test : IPamelloModule
{
    public string Name => "Test";
    public string Author => "Marsoau";
    public string Description => "Test module";
    public ELoadingStage Stage => ELoadingStage.Default;
    
    public void Startup(IServiceProvider services) {
        var peql = services.GetRequiredService<IEntityQueryService>();
        var users = services.GetRequiredService<IPamelloUserRepository>();
        var logger = services.GetRequiredService<IPamelloLogger>();

        Console.WriteLine(users.GetRequired(1).Token);
        
        logger.Log("G");
        var songs = peql.Get<IPamelloSong>("test(434)", users.GetRequired(1));
        logger.Log("G");

        foreach (var song in songs) {
            Console.WriteLine($"| {song}");
        }
    }
}
