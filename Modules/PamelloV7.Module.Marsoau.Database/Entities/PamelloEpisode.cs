using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Dto;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.DTO;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Module.Marsoau.Database.Entities.Base;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Entities;

public class PamelloEpisode : PamelloDatabaseEntity<DatabaseEpisode>, IPamelloEpisode
{
    private string _name;
    private AudioTime _start;
    private bool _autoSkip;
    private IPamelloSong _song;

    public override string Name {
        get => _name;
        protected set => throw new NotImplementedException();
    }
    public override string SetName(string name, IPamelloUser scopeUser) {
        if (_name == name) return _name;

        _name = name;
        _sink.Invoke(scopeUser, new EpisodeNameUpdated() {
            Episode = this,
            NewName = _name
        });

        Save();
        
        return _name;
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

    public override void SaveInternal() {
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

    public override IPamelloDTO GetDto() {
        return new PamelloEpisodeDTO {
            Id = Id,
            Name = Name,
            Start = Start.TotalSeconds,
            AutoSkip = AutoSkip,
            
            SongId = Song.Id,
        };
    }
}
