using PamelloV7.Server.Attributes;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Audio;
using PamelloV7.Server.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

            //WriteAllCommands();
        }

        private void WriteAllCommands() {
            var methods = typeof(PamelloCommandsModule).GetMethods();


            Console.WriteLine("Commnds:\n");
            foreach (var method in methods) {
                if (!method.CustomAttributes.Any(attribure => attribure.AttributeType == typeof(PamelloCommandAttribute))) continue;

                var parameters = method.GetParameters();

                Console.Write($"public async {method.ReturnType.ShortDisplayName()} {method.Name}(");

                foreach (var parameter in parameters) {
                    Console.Write($"{parameter.ParameterType.ShortDisplayName()} {parameter.Name}");
                }

                Console.Write($") {{\n\treturn await InvokeCommand<object>($\"{method.Name}&");

                var first = true;
                foreach (var parameter in parameters) {
                    if (!first) Console.Write("&");
                    else first = false;

                    Console.Write($"{parameter.Name}={{{parameter.Name}}}");
                }

                Console.WriteLine("\");\n}");
            }
        }

        //player
        [PamelloCommand]
        public async Task<PamelloPlayer> PlayerCreate(string playerName) {
            var player = _players.Create(User, playerName);
            User.SelectedPlayer = player;

            var vc = _discordClients.GetUserVoiceChannel(User);
            if (vc is null) return player;

            var vcUsers = _discordClients.GetVoiceChannelUsers(vc);
            foreach (var user in vcUsers) {
                if (user.SelectedPlayer is null) {
                    user.SelectedPlayer = player;
                }
            }

            return player;
        }

        [PamelloCommand]
        public async Task PlayerSelect(PamelloPlayer? player) {
            if (player is null) {
                User.SelectedPlayer = null;
                return;
            }

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

        [PamelloCommand]
        public async Task PlayerGoToEpisode(int episodePosition) {
            if (Player.Queue.Current is null) throw new PamelloException("There is no song to rewind");
            await Player.Queue.Current.RewindToEpisode(episodePosition);
        }

        [PamelloCommand]
        public async Task PlayerPrevEpisode() {
            if (Player.Queue.Current is null) throw new PamelloException("There is no song to rewind");

            var currentEpisode = Player.Queue.Current.GetCurrentEpisodePosition() ?? 0;
            await Player.Queue.Current.RewindToEpisode(currentEpisode - 1, false);
        }

        [PamelloCommand]
        public async Task PlayerNextEpisode() {
            if (Player.Queue.Current is null) throw new PamelloException("There is no song to rewind");

            var currentEpisode = Player.Queue.Current.GetCurrentEpisodePosition() ?? 0;
            await Player.Queue.Current.RewindToEpisode(currentEpisode + 1, false);
        }

        [PamelloCommand]
        public async Task PlayerRewind(int seconds) {
            if (Player.Queue.Current is null) throw new PamelloException("There is no song to rewind");
            await Player.Queue.Current.RewindTo(new AudioTime(seconds));
        }

        [PamelloCommand]
        public async Task PlayerQueueSongAdd(PamelloSong song) {
            Player.Queue.AddSong(song, User);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongInsert(int queuePosition, PamelloSong song) {
            Player.Queue.InsertSong(queuePosition, song, User);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongYoutubeAdd(string youtubeId) {
            var song = await _songs.AddAsync(youtubeId, User);
            if (song is null) return;

            Player.Queue.AddSong(song, User);
        }
        [PamelloCommand]
        public async Task PlayerQueueSongYoutubeInsert(int queuePosition, string youtubeId) {
            var song = await _songs.AddAsync(youtubeId, User);
            if (song is null) return;

            Player.Queue.InsertSong(queuePosition, song, User);
        }
        [PamelloCommand]
        public async Task PlayerQueuePlaylistAdd(PamelloPlaylist playlist) {
            Player.Queue.AddPlaylist(playlist, User);
        }
        [PamelloCommand]
        public async Task PlayerQueuePlaylistInsert(int queuePosition, PamelloPlaylist playlist) {
            Player.Queue.InsertPlaylist(queuePosition, playlist, User);
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
        public async Task SongRename(PamelloSong song, string newName) {
            song.Name = newName;
        }

        [PamelloCommand]
        public async Task SongFavoriteAdd(PamelloSong song) {
            User.AddFavoriteSong(song);
        }
        [PamelloCommand]
        public async Task SongFavoriteRemove(PamelloSong song) {
            User.RemoveFavoriteSong(song);
        }

        [PamelloCommand]
        public async Task SongAssociacionsAdd(PamelloSong song, string associacion) {
            song.AddAssociacion(associacion);
        }
        [PamelloCommand]
        public async Task SongAssociacionsRemove(PamelloSong song, string associacion) {
            song.RemoveAssociacion(associacion);
        }

        [PamelloCommand]
        public async Task<int> SongEpisodesAdd(PamelloSong song, int episodeStart, string episodeName) {
            return song.AddEpisode(new AudioTime(episodeStart), episodeName, false).Id;
        }
        [PamelloCommand]
        public async Task SongEpisodesRemove(PamelloSong song, int episodePosition) {
            song.RemoveEpisodeAt(episodePosition);
        }
        [PamelloCommand]
        public async Task SongEpisodesRename(PamelloSong song, int episodePosition, string newName) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.Name = newName;
        }
        [PamelloCommand]
        public async Task SongEpisodesSkipSet(PamelloSong song, int episodePosition, bool newState) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.AutoSkip = newState;
        }
        [PamelloCommand]
        public async Task SongEpisodesEditTime(PamelloSong song, int episodePosition, int newTime) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.Start = new AudioTime(newTime);
        }
        [PamelloCommand]
        public async Task SongEpisodesClear(PamelloSong song) {
            _episodes.DeleteAllFrom(song);
        }
    
        //playlist
        [PamelloCommand]
        public async Task<int> PlaylistCreate(string name, bool fillWithQueue) {
            var playlist = User.CreatePlaylist(name);

            if (fillWithQueue) {
                playlist.AddList(Player.Queue.Songs);
            }

            return playlist.Id;
        }
        [PamelloCommand]
        public async Task PlaylistAddSong(PamelloPlaylist playlist, PamelloSong song) {
            playlist.AddSong(song);
        }
        [PamelloCommand]
        public async Task<int> PlaylistAddPlaylistSongs(int toPlaylistId, int fromPlaylistId) {
            var toPlaylist = _playlists.GetRequired(toPlaylistId);
            var fromPlaylist = _playlists.GetRequired(fromPlaylistId);

            return toPlaylist.AddList(fromPlaylist.Songs);
        }
        [PamelloCommand]
        public async Task PlaylistRemoveSong(PamelloPlaylist playlist, PamelloSong song) {
            playlist.RemoveSong(song);
        }
        [PamelloCommand]
        public async Task PlaylistRename(PamelloPlaylist playlist, string newName) {
            playlist.Name = newName;
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteAdd(PamelloPlaylist playlist) {
            User.AddFavoritePlaylist(playlist);
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteRemove(PamelloPlaylist playlist) {
            User.RemoveFavoritePlaylist(playlist);
        }
        [PamelloCommand]
        public async Task PlaylistDelete(PamelloPlaylist playlist) {
            _playlists.Delete(playlist);
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
