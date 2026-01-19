using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Server.Entities.Base;

namespace PamelloV7.Module.Marsoau.Base.Entities;

public class PamelloEpisode : PamelloEntity<DatabaseEpisode>, IPamelloEpisode
{
    private string _name;
    private AudioTime _start;
    private bool _autoSkip;
    private IPamelloSong _song;

    public override string Name {
        get => _name;
        set => _name = value;
    }

    public AudioTime Start {
        get => _start;
        set => _start = value;
    }

    public bool AutoSkip {
        get => _autoSkip;
        set => _autoSkip = value;
    }

    public IPamelloSong Song => _song;
    
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

    public override string ToString() {
        return $"[{Id}] {Name} ({Start})";
    }
}
