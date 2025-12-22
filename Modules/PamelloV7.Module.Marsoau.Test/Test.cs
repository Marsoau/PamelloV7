using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Plugins;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.Test;

public class Test : IPamelloModule
{
    public string Name => "Test";
    public string Author => "Marsoau";
    public string Description => "Test module";
    public ELoadingStage Stage => ELoadingStage.Default;
    
    public void Startup(IServiceProvider services) {
        var peql = services.GetRequiredService<IEntityQueryService>();
        var logger = services.GetRequiredService<IPamelloLogger>();
        
        logger.Log("G");
        var song = peql.GetSingle<IPamelloSong>("1", null);
        logger.Log("G");

        Console.WriteLine($"GOT SONG: {song}");
    }
}
