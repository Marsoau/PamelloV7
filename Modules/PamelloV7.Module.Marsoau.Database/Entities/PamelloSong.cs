using System.Diagnostics;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Module.Marsoau.Base.Repositories.Database;
using PamelloV7.Module.Marsoau.Database.Entities.Base;
using PamelloV7.Module.Marsoau.Database.Repositories;

namespace PamelloV7.Module.Marsoau.Database.Entities;

public class PamelloSong : PamelloDatabaseEntity<DatabaseSong>, IPamelloSong
{
    private string _name;
    
    public List<SongSource> _sources;
    
    public List<IPamelloUser> _favoriteBy = null!;
    public List<IPamelloEpisode> _songEpisodes = null!;
    public List<IPamelloPlaylist> _songPlaylists = null!;
    public List<string> _associations;

    public override string Name => _name;

    public override bool IsDeleted { get; set; }

    public override string SetName(string value, IPamelloUser? scopeUser) {
        if (_name == value) return _name;

        _name = value;
        _sink.Invoke(scopeUser, new SongNameUpdated {
            Song = this,
            Name = _name
        });

        Save();
        
        return _name;
    }

    public string CoverUrl { get; private set; }

    public string SetCoverUrl(string value, IPamelloUser? scopeUser) {
        if (CoverUrl == value) return CoverUrl;

        CoverUrl = value;
        
        //todo cover update event

        Save();
        
        return CoverUrl;
    }

    public DateTime AddedAt { get; }

    public int SelectedSourceIndex { get; private set; }
    public void SelectSource(int index, IPamelloUser? scopeUser) {
        if (index == SelectedSourceIndex) return;
            
        SelectedSourceIndex = index;

        _sink.Invoke(scopeUser, new SongSelectedSourceIndexUpdated() {
            Song = this,
            SelectedSourceIndex = SelectedSourceIndex
        });
    }

    public bool IsDownloaded => SelectedSource?.IsDownloaded() ?? false;
    public IPamelloUser? AddedBy { get; private set; }
    
    public SongSource? SelectedSource => _sources.ElementAtOrDefault(SelectedSourceIndex);
    
    public IReadOnlyList<SongSource> Sources => _sources;
    
    public IReadOnlyList<IPamelloUser> FavoriteBy => _favoriteBy;
    public IReadOnlyList<IPamelloEpisode> Episodes => _songEpisodes;
    public IReadOnlyList<IPamelloPlaylist> Playlists => _songPlaylists;
    public IReadOnlyList<string> Associations => _associations;
    
    public PamelloSong(DatabaseSong databaseEntity, IServiceProvider services) : base(databaseEntity, services) {
        _name = databaseEntity.Name;
        CoverUrl = databaseEntity.CoverUrl;
        AddedAt = databaseEntity.AddedAt;
        _associations = databaseEntity.Associations.ToList();
        
        SelectedSourceIndex = databaseEntity.SelectedSource;
        _sources = databaseEntity.Sources.Select(
            pk => new SongSource(services, this, pk)
        ).ToList();
    }
    
    protected override void InitBase() {
        var databasePlaylists = ((PamelloPlaylistRepository)_playlists).GetCollection().GetAll();
        var databaseEpisodes = ((PamelloEpisodeRepository)_episodes).GetCollection().GetAll();
        var databaseUsers = ((PamelloUserRepository)_users).GetCollection().GetAll();
        
        AddedBy = _users.Get(DatabaseEntity.AddedBy)!;
        
        _favoriteBy = databaseUsers
            .Where(databaseUser => databaseUser.FavoriteSongIds.Contains(Id))
            .Select(databaseUser => _users.Get(databaseUser.Id))
            .OfType<IPamelloUser>()
            .ToList();
        _songEpisodes = databaseEpisodes.
            Where(databaseEpisode => databaseEpisode.SongId == Id)
            .Select(databaseEpisode => _episodes.Get(databaseEpisode.Id))
            .OfType<IPamelloEpisode>()
            .ToList();
        _songPlaylists = databasePlaylists.
            Where(databasePlaylist => databasePlaylist.SongIds.Contains(Id))
            .Select(databasePlaylist => _playlists.Get(databasePlaylist.Id))
            .OfType<IPamelloPlaylist>()
            .ToList();
        
        _songEpisodes.Sort((a, b) => {
            if (a.Start.TotalSeconds == b.Start.TotalSeconds) return 0;
            return a.Start.TotalSeconds > b.Start.TotalSeconds ? 1 : -1;
        });
    }

    public override void SaveInternal() {
        var songCollection = ((PamelloSongRepository)_songs).GetCollection();
        
        var databaseSong = songCollection.Get(Id);
        if (databaseSong is null) return;
        //Debug.Assert(databaseSong is not null, "Song doesnt exist in the database for some reason, cant save song");
        
        databaseSong.Name = _name;
        databaseSong.CoverUrl = CoverUrl;

        databaseSong.Associations = _associations;

        databaseSong.SelectedSource = SelectedSourceIndex;
        databaseSong.Sources = _sources.Select(source => source.PK).ToList();
        
        songCollection.Save(databaseSong);
    }
    
    /*
    *
    *
    *
    *
    *
    * 
    *
    *
    *
    * 
    */

    public void AddSource(SongSource source, IPamelloUser? scopeUser) {
        throw new NotImplementedException();
    }

    public bool AddAssociation(string association, IPamelloUser? scopeUser) {
        if (string.IsNullOrWhiteSpace(association)) return false;
        if (!association.All(char.IsLetter)) return false;
        
        association = association.ToLowerInvariant();
        
        var databaseAssociations = ((PamelloSongRepository)_songs)
            .GetCollection()
            .GetAll()
            .SelectMany(databaseSong => databaseSong.Associations);
        
        if (databaseAssociations.Contains(association)) return false;
        
        _associations.Add(association);

        _sink.Invoke(scopeUser, new SongAssociationsUpdated() {
            Song = this,
            Associations = _associations
        });
        
        Save();
        
        return true;
    }

    public bool RemoveAssociation(string association, IPamelloUser? scopeUser) {
        if (!_associations.Contains(association)) return false;
        
        _associations.Remove(association);

        _sink.Invoke(scopeUser, new SongAssociationsUpdated() {
            Song = this,
            Associations = _associations
        });
        
        Save();
        
        return true;
    }

    public void MakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false) {
        if (_favoriteBy.Contains(user)) return;
        
        _favoriteBy.Add(user);
        
        if (!fromInside) user.AddFavoriteSong(this, null, true);

        _sink.Invoke(automatic ? null : user, new SongFavoriteByUpdated() {
            Song = this,
            FavoriteBy = IPamelloEntity.GetIds(_favoriteBy)
        });
        
        Save();
    }

    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false) {
        if (!_favoriteBy.Remove(user)) return;
        
        if (!fromInside) user.RemoveFavoriteSong(this, true);

        _sink.Invoke(automatic ? null : user, new SongFavoriteByUpdated() {
            Song = this,
            FavoriteBy = IPamelloEntity.GetIds(_favoriteBy)
        });
        
        Save();
    }

    public IPamelloEpisode AddEpisode(AudioTime start, string name, bool autoSkip, IPamelloUser scopeUser) {
        return _episodes.Add(start, name, autoSkip, this, scopeUser);
    }

    public IPamelloEpisode AddEpisode(IEpisodeInfo episodeInfo, bool autoSkip, IPamelloUser scopeUser) {
        return AddEpisode(new AudioTime(episodeInfo.Start), episodeInfo.Name, autoSkip, scopeUser);
    }

    public void RemoveEpisode(IPamelloEpisode episode, IPamelloUser scopeUser) {
        _episodes.Delete(episode, scopeUser);
    }

    public void RemoveEpisodeAt(int position, IPamelloUser scopeUser) {
        var episode = _songEpisodes.ElementAtOrDefault(position);
        if (episode is null) return;
        
        _episodes.Delete(episode, scopeUser);
    }

    public void RemoveAllEpisodes(IPamelloUser scopeUser) {
        _episodes.DeleteAllFrom(this, scopeUser);
    }

    public IPamelloPlaylist AddToPlaylist(IPamelloPlaylist playlist, IPamelloUser? scopeUser, int? position = null, bool fromInside = false) {
        if (!_songPlaylists.Contains(playlist)) _songPlaylists.Add(playlist);
        
        if (!fromInside) playlist.AddSongs([this], scopeUser, position, fromInside);
        
        //todo playlist update event

        Save();
        
        return playlist;
    }

    public void RemoveFromPlaylist(IPamelloPlaylist playlist, IPamelloUser? scopeUser, bool fromInside = false) {
        if (!_songPlaylists.Remove(playlist)) return;
        
        //todo playlist update event

        if (!fromInside) playlist.RemoveSong(this, scopeUser, fromInside);
    }

    public override PamelloEntityDto GetDto() {
        return new PamelloSongDto {
            Id = Id,
            Name = Name,
            CoverUrl = CoverUrl,
            Associations = _associations,
            AddedAt = AddedAt,
            AddedBy = AddedBy?.Id ?? 0,

            FavoriteBy = IPamelloEntity.GetIds(FavoriteBy),
            Playlists = IPamelloEntity.GetIds(Playlists),
            Episodes = IPamelloEntity.GetIds(Episodes),

            SelectedSourceIndex = SelectedSourceIndex,
            SourcesPlatformKeys = Sources.Select(source => source.PK.ToString())
        };
    }
}
