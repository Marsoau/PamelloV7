using System.Diagnostics;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Entities.Base;
using PamelloV7.Server.Repositories.Database;

namespace PamelloV7.Server.Entities;

public class PamelloEpisode : PamelloEntity<DatabaseEpisode>, IPamelloEpisode
{
    private string _name;
    private AudioTime _start;
    private bool _autoSkip;
    private IPamelloSong _song;
    
    public override string Name { get; set; }

    public AudioTime Start { get; set; }
    public bool AutoSkip { get; set; }
    public IPamelloSong Song { get; }
    
    public PamelloEpisode(DatabaseEpisode databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _name = databaseEntity.Name;
        _start = new AudioTime(databaseEntity.StartSeconds);
        _autoSkip = databaseEntity.AutoSkip;
    }

    protected override void InitBase() {
        var songs = _services.GetRequiredService<IPamelloSongRepository>();
        
        _song = songs.Get(_databaseEntity.SongId)!;
    }

    public override void Save() {
        var episodeCollection = ((PamelloEpisodeRepository)_episodes).GetCollection();
        
        var databaseEpisode = episodeCollection.Get(Id);
        Debug.Assert(databaseEpisode is not null, "Episode doesnt exist in the database for some reason, cant save episode");
        
        databaseEpisode.Name = Name;
        databaseEpisode.StartSeconds = Start.TotalSeconds;
        databaseEpisode.AutoSkip = AutoSkip;
        
        episodeCollection.Save(databaseEpisode);
    }
}
