using PamelloV7.Server.Attributes;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Audio;
using PamelloV7.Server.Services;
using Discord.WebSocket;

namespace PamelloV7.Server.Modules
{
    public class PamelloCommandsModule
    {
        private readonly IServiceProvider _services;

        private readonly DiscordClientService _discordClients;

        private readonly PamelloPlayerRepository _players;
        private readonly PamelloSpeakerService _speakers;

        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly PamelloEpisodeRepository _episodes;
        private readonly PamelloPlaylistRepository _playlists;

        public PamelloUser User { get; }
        private PamelloPlayer Player {
            get => User.RequiredSelectedPlayer;
        }

        public PamelloCommandsModule(IServiceProvider services, PamelloUser user) {
            _services = services;

            _discordClients = services.GetRequiredService<DiscordClientService>();

            _players = services.GetRequiredService<PamelloPlayerRepository>();
            _speakers = services.GetRequiredService<PamelloSpeakerService>();

            _users = services.GetRequiredService<PamelloUserRepository>();
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _episodes = services.GetRequiredService<PamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<PamelloPlaylistRepository>();

            User = user;
        }

        //player
        [PamelloCommand]
        public async Task<int> PlayerCreate(string playerName) {
            var player = _players.Create(User, playerName);
            User.SelectedPlayer = player;

            return player.Id;
        }
        [PamelloCommand]
        public async Task PlayerSelect(int? playerId) {
            if (playerId is null) {
                User.SelectedPlayer = null;
                return;
            }

            var player = _players.GetRequired(playerId.Value);

            if (player.IsProtected) {
                var vc = _discordClients.GetUserVoiceChannel(User);
                if (player.Creator.Id == User.Id) {
                    User.SelectedPlayer = player;
                    return;
                }
                if (vc is not null) {
                    var vcPlayers = _speakers.GetVoicePlayers(vc.Id);
                    if (vcPlayers.Contains(player)) {
                        User.SelectedPlayer = player;
                        return;
                    }
                }

                throw new PamelloException("Cant select protected player");
            }
            else {
                User.SelectedPlayer = player;
            }
        }
        [PamelloCommand]
        public async Task PlayerProtection(bool state) {
            if (Player.Creator != User) {
                throw new PamelloException("Only creator of the player can set it protection");
            }

            Player.IsProtected = state;
        }
        [PamelloCommand]
        public async Task PlayerDelete() {
            throw new NotImplementedException();
        }
        
        [PamelloCommand]
        public async Task PlayerResume() {
            Player.IsPaused = false;
        }
        [PamelloCommand]
        public async Task PlayerPause() {
            Player.IsPaused = true;
        }

        [PamelloCommand]
        public async Task<int?> PlayerSkip() {
            var song = Player.Queue.Current?.Song;

            Player.Queue.GoToNextSong();

            return song?.Id;
        }
        [PamelloCommand]
        public async Task<int> PlayerGoTo(int songPosition, bool returnBack) {
            return Player.Queue.GoToSong(songPosition, returnBack).Id;
        }
        [PamelloCommand]
        public async Task<int> PlayerPrev() {
            return Player.Queue.GoToSong(Player.Queue.Position - 1).Id;
        }
        [PamelloCommand]
        public async Task<int> PlayerNext() {
            return Player.Queue.GoToSong(Player.Queue.Position + 1).Id;
        }
        public async Task PlayerGoToEpisode(int episodePosition) {
            Player.Queue.Current?.RewindToEpisode(episodePosition);
        }
        public async Task PlayerPrevEpisode() {
            var currentEpisode = Player.Queue.Current?.GetCurrentEpisodePosition();
            if (currentEpisode is null || Player.Queue.Current is null) return;

            await Player.Queue.Current.RewindToEpisode(currentEpisode.Value - 1);
        }
        public async Task PlayerNextEpisode() {
            var currentEpisode = Player.Queue.Current?.GetCurrentEpisodePosition();
            if (currentEpisode is null || Player.Queue.Current is null) return;

            await Player.Queue.Current.RewindToEpisode(currentEpisode.Value + 1);
        }
        public async Task PlayerRewind(int seconds) {
            Player.Queue.Current?.RewindTo(new AudioTime(seconds));
        }

        [PamelloCommand]
        public async Task PlayerQueueSongAdd(int songId) {
            var song = _songs.GetRequired(songId);
            Player.Queue.AddSong(song);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongInsert(int queuePosition, int songId) {
            var song = _songs.GetRequired(songId);
            Player.Queue.InsertSong(queuePosition, song);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongYoutubeAdd(string youtubeId) {
            var song = await _songs.AddAsync(youtubeId, User);
            if (song is null) return;

            Player.Queue.AddSong(song);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongYoutubeInsert(int queuePosition, string youtubeId) {
            var song = await _songs.AddAsync(youtubeId, User);
            if (song is null) return;

            Player.Queue.InsertSong(queuePosition, song);
        }
        [PamelloCommand]
        public async Task PlayerQueuePlaylistAdd(int playlistId) {
            var playlist = _playlists.GetRequired(playlistId);
            Player.Queue.AddPlaylist(playlist);
        }
        [PamelloCommand]
        public async Task PlayerQueuePlaylistInsert(int queuePosition, int playlistId) {
            var playlist = _playlists.GetRequired(playlistId);
            Player.Queue.InsertPlaylist(queuePosition, playlist);
        }
        [PamelloCommand]
        public async Task<int> PlayerQueueSongRemove(int position) {
            return Player.Queue.RemoveSong(position).Id;
        }
        [PamelloCommand]
        public async Task PlayerQueueSongSwap(int inPosition, int withPosition) {
            Player.Queue.SwapSongs(inPosition, withPosition);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongMove(int fromPosition, int toPosition) {
            Player.Queue.MoveSong(fromPosition, toPosition);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongRequestNext(int? position) {
            Player.Queue.NextPositionRequest = position;
        }

        [PamelloCommand]
        public async Task PlayerQueueRandom(bool value) {
            Player.Queue.IsRandom = value;
        }
        [PamelloCommand]
        public async Task PlayerQueueReversed(bool value) {
            Player.Queue.IsReversed = value;
        }
        [PamelloCommand]
        public async Task PlayerQueueNoLeftovers(bool value) {
            Player.Queue.IsNoLeftovers = value;
        }
        [PamelloCommand]
        public async Task PlayerQueueFeedRandom(bool value) {
            Player.Queue.IsFeedRandom = value;
        }

        [PamelloCommand]
        public async Task PlayerQueueSuffle() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlayerQueueClear() {
            Player.Queue.Clear();
        }

        //song
        [PamelloCommand]
        public async Task<int> SongAdd(string youtubeId) {
            return (await _songs.AddAsync(youtubeId, User))?.Id ?? throw new PamelloException($"Cant add youtube song with id \"{youtubeId}\"");
        }
        [PamelloCommand]
        public async Task SongRename(int songId, string newName) {
            var song = _songs.GetRequired(songId);
            song.Name = newName;
        }

        [PamelloCommand]
        public async Task SongFavoriteAdd(int songId) {
            var song = _songs.GetRequired(songId);

            User.AddFavoriteSong(song);
        }
        [PamelloCommand]
        public async Task SongFavoriteRemove(int songId) {
            var song = _songs.GetRequired(songId);

            User.RemoveFavoriteSong(song);
        }

        [PamelloCommand]
        public async Task SongAssociacionsAdd(int songId, string associacion) {
            var song = _songs.GetRequired(songId);
            song.AddAssociacion(associacion);
        }
        [PamelloCommand]
        public async Task SongAssociacionsRemove(int songId, string associacion) {
            var song = _songs.GetRequired(songId);
            song.RemoveAssociacion(associacion);
        }

        [PamelloCommand]
        public async Task<int> SongEpisodesAdd(int songId, int episodeStart, string episodeName) {
            var song = _songs.GetRequired(songId);
            return song.AddEpisode(new AudioTime(episodeStart), episodeName).Id;
        }
        [PamelloCommand]
        public async Task SongEpisodesRemove(int songId, int episodePosition) {
            var song = _songs.GetRequired(songId);
            song.RemoveEpisode(episodePosition);
        }
        [PamelloCommand]
        public async Task SongEpisodesRename(int songId, int episodePosition, string newName) {
            var song = _songs.GetRequired(songId);
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.Name = newName;
        }
        [PamelloCommand]
        public async Task SongEpisodesSkipSet(int songId, int episodePosition, bool newState) {
            var song = _songs.GetRequired(songId);
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.Skip = newState;
        }
        [PamelloCommand]
        public async Task SongEpisodesEditTime(int songId, int episodePosition, int newTime) {
            var song = _songs.GetRequired(songId);
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.Start = newTime;
        }
        [PamelloCommand]
        public async Task SongEpisodesClear(int songId) {
            var song = _songs.GetRequired(songId);
            _episodes.DeleteAllFrom(song);
        }
    
        //playlist
        [PamelloCommand]
        public async Task PlaylistCreate() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistAddSong() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistAddPlaylistSongs() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistSearch() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistInfo() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistRename() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteAdd() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteRemove() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlaylistDelete() {
            throw new NotImplementedException();
        }

        //speakers
        [PamelloCommand]
        public async Task SpeakerConnect() {
            var vc = _discordClients.GetUserVoiceChannel(User);
            if (vc is null) throw new PamelloException("You have to be in voce channel to execute this command");

            await _speakers.ConnectSpeaker(Player, vc.Guild.Id, vc.Id);
        }
        [PamelloCommand]
        public async Task SpeakerDisconnect() {
            var vc = _discordClients.GetUserVoiceChannel(User);
            if (vc is null) throw new PamelloException("You have to be in voce channel to execute this command");

            await _speakers.DisconnectSpeaker(Player, vc.Id);
        }
    }
}
