using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;
using PamelloV7.Module.Marsoau.Database.Entities;
using PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

namespace PamelloV7.Module.Marsoau.Database.Repositories;

public class PamelloSongRepository : PamelloDatabaseRepository<IPamelloSong, DatabaseSong>, IPamelloSongRepository
{
    private readonly IPlatformService _platforms;

    public override string CollectionName => "songs";
    
    public PamelloSongRepository(IServiceProvider services) : base(services) {
        _platforms = services.GetRequiredService<IPlatformService>();
    }

    public void Startup(IServiceProvider services) {
        _events.Subscribe<SongDeleted>((scopeUser, e) => {
            var playlists = e.Song.Playlists.ToList();
            var episodes = e.Song.Episodes.ToList();
            var favoriteBy = e.Song.FavoriteBy.ToList();

            Console.WriteLine($"About to delete {episodes.Count} episodes from {e.Song}");
            
            //foreach (var playlist in playlists) playlist.RemoveSong(e.Song, scopeUser);
            foreach (var episode in episodes) _episodes.Delete(episode, scopeUser);
            //foreach (var user in favoriteBy) user.RemoveFavoriteSong(e.Song);
        });
        //we could slaos soubscribe to SongRestore in the way we would execute all inner revert packs inside the subscription
        //so all inner events are recorded as the ones inside SongRestore
        //triggering revert pack invokation propagation from this subscription will result in all underlying revert packs be executed on exit from the SongRestore pack, so it should check if it was propagated before doing so
    }

    protected override IPamelloSong CreatePamelloEntity(DatabaseSong databaseEntity) {
        return new PamelloSong(databaseEntity, _services);
    }

    public IPamelloSong? Get(IPamelloUser scopeUser, int id) {
        return Get(id);
    }

    public async Task<IPamelloSong?> GetByNameAsync(IPamelloUser scopeUser, string query) {
        if (query.Length == 0) return null;
        
        Console.WriteLine($"Get by name requested: {query}");
        
        //actual search by name here
        
        var associationSong = _loaded.FirstOrDefault(song => song.Associations.Contains(query));
        if (associationSong is not null) return associationSong;
        
        var pk = _platforms.GetSongPlatformKey(query);
        if (pk is null) return null;

        return await GetByPlatformKeyAsync(scopeUser, pk, true);
    }

    public async Task<IPamelloSong?> GetByPlatformKeyAsync(IPamelloUser scopeUser, PlatformKey pk, bool allowCreation = false) {
        var song = _loaded.FirstOrDefault(s => s.Sources.Any(source => source.PK == pk));
        Console.WriteLine($"Song by pk {(song is not null ? $"found: {song}" : "not found")}");
        if (song is not null) return song;
        
        if (!allowCreation) return null;
        
        var platform = _platforms.GetSongPlatform(pk.Platform);
        if (platform is null) return null;
        
        var songInfo = await platform.GetSongInfoAsync(pk.Key);
        Console.WriteLine($"Info by pk {(songInfo is not null ? $"found: {songInfo}" : "not found")}");
        if (songInfo is null) return null;
        
        Console.WriteLine("Adding song by info");
        return Add(songInfo, scopeUser);
    }

    public IEnumerable<IPamelloSong> GetCurrent(IPamelloUser scopeUser) {
        var queue = scopeUser.SelectedPlayer?.Queue;
        if (queue is null) return [];

        var current = queue.Entries.ElementAtOrDefault(queue.Position)?.Song;
        if (current is null) return [];

        return [current];
    }

    public IEnumerable<IPamelloSong> GetRandom(IPamelloUser scopeUser) {
        var song = _loaded.ElementAtOrDefault(Random.Shared.Next(_loaded.Count));
        return song is not null ? [song] : [];
    }

    public IEnumerable<IPamelloSong> GetQueue(IPamelloUser scopeUser) {
        var queue = scopeUser.SelectedPlayer?.Queue;
        return queue is null ? [] : queue.Entries.Select(entry => entry.Song);
    }

    public IEnumerable<IPamelloSong> GetAll(IPamelloUser scopeUser, IPamelloUser? addedBy = null, IPamelloUser? favoriteBy = null) {
        IEnumerable<IPamelloSong> results = _loaded.AsReadOnly();
        
        if (addedBy is not null) results = results.Where(s => s.AddedBy == addedBy);
        if (favoriteBy is not null) results = results.Where(s => s.FavoriteBy.Contains(favoriteBy));
        
        return results;
    }

    public IEnumerable<IPamelloSong> GetFavorite(IPamelloUser scopeUser, IPamelloUser? by) {
        return (by ?? scopeUser).FavoriteSongs;
    }

    public IEnumerable<IPamelloSong> GetFromPlaylist(IPamelloUser scopeUser, IPamelloPlaylist? playlist) {
        return playlist?.Songs ?? [];
    }

    public IEnumerable<IPamelloSong> TestPoint(IPamelloUser scopeUser, int value) {
        Console.WriteLine($"TTEEST PINT: {value}");
        return [];
    }

    public IPamelloSong Add(ISongInfo info, IPamelloUser adder) {
        var databaseSong = new DatabaseSong() {
            Name = info.Name,
            CoverUrl = info.CoverUrl,
            Associations = [],
            Sources = [
                new PlatformKey(info.Platform.Name, info.Key)
            ],
            AddedBy = adder.Id,
            AddedAt = DateTime.Now,
        };
        
        GetCollection().Add(databaseSong);
        
        var song = (PamelloSong)Load(databaseSong, false);

        song._songEpisodes = [];

        foreach (var episodeInfo in info.Episodes) {
            _episodes.Add(new AudioTime(episodeInfo.Start), episodeInfo.Name, false, song, adder);
        }
        
        song.Init();

        return song;
    }

    public IEnumerable<IPamelloSong> Search(string querry, IPamelloUser scopeUser, IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null) {
        throw new NotImplementedException("because author is gay");
    }

    public override IHistoryRecord Delete(IPamelloSong song, IPamelloUser? scopeUser) {
        var pamelloSong = (PamelloSong)song;
        
        var collection = GetCollection();
        var databaseSong = collection.Get(song.Id)!;
        
        collection.Delete(song.Id);
        _loaded.Remove(song);

        //all other objects that have a link to this song should delete it on this event
        var record = _events.Invoke(scopeUser, new SongDeleted() {
            RevertPack = new SongDeletionRevertPack() {
                DatabaseSong = databaseSong,
            },
            Song = pamelloSong,
        })!;

        foreach (var source in pamelloSong.Sources) {
            if (source.GetFile() is { Exists: true } file) file.Delete();
        }
        
        return record;
    }

    public void Restore(IPamelloUser scopeUser, DatabaseSong databaseSong) {
        var collection = GetCollection();

        if (collection.Get(databaseSong.Id) is not null) {
            throw new PamelloException("Song already exists in the database");
        }
        
        collection.Add(databaseSong);
        
        var pamelloSong = GetRequired(databaseSong.Id);

        if (NestedPamelloEvent.Current.Value is { } currentNested) {
            _events.Invoke(scopeUser, new SongRestored() {
                RevertPack = new SongRestoreRevertPack(),
                Song = pamelloSong
            }, () => {
                currentNested.Propagate(scopeUser);
            });
        }
        else {
            _events.Invoke(scopeUser, new SongRestored() {
                RevertPack = new SongRestoreRevertPack(),
                Song = pamelloSong
            });
        }
    }
}
