using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.Enumerators
{
    public enum EEventNameNew
    {
        //EVENTS STATUS UPDATES
        EventsConnected = 1,
        EventsAuthorized,
        EventsUnAuthorized,
        
        //SELECTED PLAYER
        PlayerNameUpdated,
        PlayerStateUpdated,
        PlayerIsPausedUpdated,
        PlayerProtectionUpdated,
        PlayerCurrentSongIdUpdated,
        PlayerQueueEntriesDTOsUpdated,
        PlayerQueuePositionUpdated,
        PlayerCurrentEpisodePositionUpdated,
        PlayerNextPositionRequestUpdated,
        PlayerCurrentSongTimePassedUpdated,
        PlayerCurrentSongTimeTotalUpdated,
        PlayerQueueIsRandomUpdated,
        PlayerQueueIsReversedUpdated,
        PlayerQueueIsNoLeftoversUpdated,
        PlayerQueueIsFeedRandomUpdated,
        
        //ENTITIES CHANGES
        //user
        UsersAvailableUpdated,
        
        UserNameUpdated,
        UserAvatarUpdated,
        UserSelectedPlayerIdUpdated,
        UserSongsPlayedUpdated,
        UserAddedSongsUpdated,
        UserAddedPlaylistsUpdated,
        UserFavoriteSongsUpdated,
        UserFavoritePlaylistsUpdated,
        UserIsAdministratorUpdated,
        
        //song
        SongsAvailableUpdated,
        
        SongNameUpdated,
        SongCoverUrlUpdated,
        SongPlayCountUpdated,
        SongAssociationsUpdated,
        SongFavoriteByIdsUpdated,
        SongEpisodesIdsUpdated,
        SongPlaylistsIdsUpdated,
        SongDownloadStarted,
        SongDownloadProgressUpdated,
        SongDownloadFinished,
        
        //episode
        EpisodesAvailableUpdated,
        
        EpisodeNameUpdated,
        EpisodeStartUpdated,
        EpisodeSkipUpdated,
        
        //playlist
        PlaylistsAvailableUpdated,
        
        PlaylistNameUpdated,
        PlaylistProtectionUpdated,
        PlaylistSongsUpdated,
        PlaylistFavoriteByIdsUpdated,
        
        //speaker
        SpeakersAvailableUpdated,
        
        //SERVER MESSAGES
    }
    public enum EEventName
    {
        EventsConnected,
        EventsAuthorized,
        EventsUnAuthorized,

        UserCreated,
        UserUpdated,
        UserNameUpdated,
        UserAvatarUpdated,
        UserSelectedPlayerIdUpdated,
        UserSongsPlayedUpdated,
        UserAddedSongsUpdated,
        UserAddedPlaylistsUpdated,
        UserFavoriteSongsUpdated,
        UserFavoritePlaylistsUpdated,
        UserIsAdministratorUpdated,

        SongCreated,
        SongUpdated,
        SongNameUpdated,
        SongCoverUrlUpdated,
        SongPlayCountUpdated,
        SongAssociacionsUpdated,
        SongFavoriteByIdsUpdated,
        SongEpisodesIdsUpdated,
        SongPlaylistsIdsUpdated,
        SongDownloadStarted,
        SongDownloadProgeressUpdated,
        SongDownloadFinished,

        EpisodeCreated,
        EpisodeUpdated,
        EpisodeDeleted,
        EpisodeNameUpdated,
        EpisodeStartUpdated,
        EpisodeSkipUpdated,

        PlaylistCreated,
        PlaylistUpdated,
        PlaylistDeleted,
        PlaylistNameUpdated,
        PlaylistProtectionUpdated,
        PlaylistSongsUpdated,
        PlaylistFavoriteByIdsUpdated,

        PlayerAvailable,
        PlayerRemoved,
        PlayerUpdated,
        PlayerNameUpdated,
        PlayerStateUpdated,
        PlayerIsPausedUpdated,
        PlayerProtectionUpdated,
        PlayerCurrentSongIdUpdated,
        PlayerQueueEntriesDTOsUpdated,
        PlayerQueuePositionUpdated,
        PlayerCurrentEpisodePositionUpdated,
        PlayerNextPositionRequestUpdated,
        PlayerCurrentSongTimePassedUpdated,
        PlayerCurrentSongTimeTotalUpdated,
        PlayerQueueIsRandomUpdated,
        PlayerQueueIsReversedUpdated,
        PlayerQueueIsNoLeftoversUpdated,
        PlayerQueueIsFeedRandomUpdated,
    }
}
