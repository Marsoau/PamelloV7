using PamelloV7.Core.Attributes;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Core.Audio;
using PamelloV7.Server.Services;
using PamelloV7.Core;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;

namespace PamelloV7.Server.Modules
{
    public class PamelloCommandsModule : IPamelloCommandsModule
    {
        private readonly IServiceProvider _services;

        private readonly DiscordClientService _discordClients;

        private readonly IPamelloPlayerRepository _players;
        private readonly IPamelloSpeakerRepository _speakers;

        private readonly IPamelloUserRepository _users;
        private readonly IPamelloSongRepository _songs;
        private readonly IPamelloEpisodeRepository _episodes;
        private readonly IPamelloPlaylistRepository _playlists;

        public IPamelloUser User { get; }
        private IPamelloPlayer Player {
            get => User.RequiredSelectedPlayer;
        }

        public PamelloCommandsModule(IServiceProvider services, IPamelloUser user) {
            _services = services;

            _discordClients = services.GetRequiredService<DiscordClientService>();

            _players = services.GetRequiredService<IPamelloPlayerRepository>();
            _speakers = services.GetRequiredService<IPamelloSpeakerRepository>();

            _users = services.GetRequiredService<IPamelloUserRepository>();
            _songs = services.GetRequiredService<IPamelloSongRepository>();
            _episodes = services.GetRequiredService<IPamelloEpisodeRepository>();
            _playlists = services.GetRequiredService<IPamelloPlaylistRepository>();

            User = user;

            //WriteAllCommands();
        }

        private void WriteAllCommands() {
            var methods = typeof(PamelloCommandsModule).GetMethods();


            Console.WriteLine("Commnds:\n");
            foreach (var method in methods) {
                if (!method.CustomAttributes.Any(attribure => attribure.AttributeType == typeof(PamelloCommandAttribute))) continue;

                var parameters = method.GetParameters();

                Console.Write($"public async {method.ReturnType.Name} {method.Name}(");

                foreach (var parameter in parameters) {
                    Console.Write($"{parameter.ParameterType.Name} {parameter.Name}");
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
        public async Task<IPamelloPlayer> PlayerCreate(string playerName) {
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
        public async Task<IPamelloPlayer?> PlayerSelect(IPamelloPlayer? player) {
            User.RequireSelectPlayer(player);
            
            return User.SelectedPlayer;
        }
        [PamelloCommand]
        public async Task<bool> PlayerProtection(bool state) {
            if (Player.Creator != User) {
                throw new PamelloException("Only creator of the player can change it protection");
            }

            return Player.IsProtected = state;
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
        public async Task<IPamelloSong?> PlayerSkip() {
            var song = Player.Queue.Audio?.Song;

            Player.Queue.GoToNextSong();

            return song;
        }
        [PamelloCommand]
        public async Task<IPamelloSong> PlayerGoTo(int songPosition, bool returnBack) {
            return Player.Queue.GoToSong(songPosition, returnBack);
        }
        [PamelloCommand]
        public async Task<IPamelloSong> PlayerPrev() {
            return Player.Queue.GoToSong(Player.Queue.Position - 1);
        }
        [PamelloCommand]
        public async Task<IPamelloSong> PlayerNext() {
            return Player.Queue.GoToSong(Player.Queue.Position + 1);
        }

        [PamelloCommand]
        public async Task<IPamelloEpisode?> PlayerGoToEpisode(int episodePosition) {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");
            return await Player.Queue.Audio.RewindToEpisode(episodePosition);
        }

        [PamelloCommand]
        public async Task<IPamelloEpisode?> PlayerPrevEpisode() {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");

            var currentEpisode = Player.Queue.Audio.GetCurrentEpisodePosition() ?? 0;
            return await Player.Queue.Audio.RewindToEpisode(currentEpisode - 1, false);
        }

        [PamelloCommand]
        public async Task<IPamelloEpisode?> PlayerNextEpisode() {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");

            var currentEpisode = Player.Queue.Audio.GetCurrentEpisodePosition() ?? 0;
            return await Player.Queue.Audio.RewindToEpisode(currentEpisode + 1, false);
        }

        [PamelloCommand]
        public async Task PlayerRewind(int seconds) {
            if (Player.Queue.Audio is null) throw new PamelloException("There is no song to rewind");
            await Player.Queue.Audio.RewindTo(new AudioTime(seconds), true, CancellationToken.None);
        }

        [PamelloCommand]
        public async Task<IPamelloSong> PlayerQueueSongAdd(IPamelloSong song, int? position = null) {
            return position is null ?
                Player.Queue.AddSong(song, User) :
                Player.Queue.InsertSong(position.Value, song, User);
        }
        
        [PamelloCommand]
        public async Task<IPamelloPlaylist> PlayerQueuePlaylistAdd(IPamelloPlaylist playlist, int? position = null) {
            return position is null ?
                Player.Queue.AddPlaylist(playlist, User) :
                Player.Queue.InsertPlaylist(position.Value, playlist, User);
        }
        
        [PamelloCommand]
        public async Task<IPamelloSong> PlayerQueueSongRemove(int position) {
            return Player.Queue.RemoveSong(position);
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
        public async Task PlayerQueueShuffle() {
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task PlayerQueueClear() {
            Player.Queue.Clear();
        }

        //song
        [PamelloCommand]
        public async Task<IPamelloSong> SongAdd(string youtubeId) {
            //return await _songs.AddAsync(youtubeId, User) ?? throw new PamelloException($"Cant add youtube song with id \"{youtubeId}\"");
            throw new NotImplementedException();
        }
        [PamelloCommand]
        public async Task<string> SongRename(IPamelloSong song, string newName) {
            return song.Name = newName;
        }

        [PamelloCommand]
        public async Task SongFavoriteAdd(IPamelloSong song) {
            User.AddFavoriteSong(song);
        }
        [PamelloCommand]
        public async Task SongFavoriteRemove(IPamelloSong song) {
            User.RemoveFavoriteSong(song);
        }

        [PamelloCommand]
        public async Task SongAssociationsAdd(IPamelloSong song, string associacion) {
            song.AddAssociation(associacion);
        }
        [PamelloCommand]
        public async Task SongAssociationsRemove(IPamelloSong song, string associacion) {
            song.RemoveAssociation(associacion);
        }

        [PamelloCommand]
        public async Task<IPamelloEpisode> SongEpisodeAdd(IPamelloSong song, int episodeStart, string episodeName) {
            return song.AddEpisode(new AudioTime(episodeStart), episodeName, false);
        }
        [PamelloCommand]
        public async Task SongEpisodeRemove(IPamelloSong song, int episodePosition) {
            song.RemoveEpisodeAt(episodePosition);
        }
        [PamelloCommand]
        public async Task<string> SongEpisodeRename(IPamelloSong song, int episodePosition, string newName) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            return episode.Name = newName;
        }
        [PamelloCommand]
        public async Task SongEpisodeSkipSet(IPamelloSong song, int episodePosition, bool newState) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.AutoSkip = newState;
        }
        [PamelloCommand]
        public async Task<int> SongEpisodeEditTime(IPamelloSong song, int episodePosition, int newTime) {
            var episode = song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) throw new PamelloException($"cant find episode in position {episodePosition}");

            episode.Start = new AudioTime(newTime);
            return episode.Start.TotalSeconds;
        }
        [PamelloCommand]
        public async Task SongEpisodesClear(IPamelloSong song) {
            _episodes.DeleteAllFrom(song);
        }
    
        //playlist
        [PamelloCommand]
        public async Task<IPamelloPlaylist> PlaylistCreate(string name, bool fillWithQueue) {
            var playlist = User.CreatePlaylist(name);

            if (fillWithQueue) {
                playlist.AddList(Player.Queue.Songs);
            }

            return playlist;
        }
        [PamelloCommand]
        public async Task<IPamelloSong?> PlaylistAddSong(IPamelloPlaylist playlist, IPamelloSong song, int? position = null) {
            return playlist.AddSong(song, position);
        }
        [PamelloCommand]
        public async Task PlaylistAddPlaylistSongs(IPamelloPlaylist fromPlaylist, IPamelloPlaylist toPlaylist, int? position = null) {
            toPlaylist.AddList(fromPlaylist.Songs, position);
        }
        [PamelloCommand]
        public async Task<int> PlaylistRemoveSong(IPamelloPlaylist playlist, IPamelloSong song) {
            return playlist.RemoveSong(song);
        }
        [PamelloCommand]
        public async Task<IPamelloSong?> PlaylistRemoveAt(IPamelloPlaylist playlist, int position) {
            return playlist.RemoveAt(position);
        }
        [PamelloCommand]
        public async Task<IPamelloSong?> PlaylistMoveSong(IPamelloPlaylist playlist, int fromPosition, int toPosition) {
            return playlist.MoveSong(fromPosition, toPosition);
        }
        [PamelloCommand]
        public async Task<string> PlaylistRename(IPamelloPlaylist playlist, string newName) {
            return playlist.Name = newName;
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteAdd(IPamelloPlaylist playlist) {
            User.AddFavoritePlaylist(playlist);
        }
        [PamelloCommand]
        public async Task PlaylistFavoriteRemove(IPamelloPlaylist playlist) {
            User.RemoveFavoritePlaylist(playlist);
        }
        [PamelloCommand]
        public async Task PlaylistDelete(IPamelloPlaylist playlist) {
            _playlists.Delete(playlist);
        }
        
        //speakers
        /*
        [PamelloCommand]
        public async Task<IPamelloDiscordSpeaker> SpeakerDiscordConnect() {
            var vc = _discordClients.GetUserVoiceChannel(User);
            if (vc is null) throw new PamelloException("You have to be in voce channel to execute this command");

            return await _speakers.ConnectDiscord(Player, vc.Guild.Id, vc.Id);
        }
        */
        [PamelloCommand]
        public async Task<IPamelloInternetSpeaker> SpeakerInternetConnect(string? name) {
            var speaker = await _speakers.ConnectInternet(Player, name);

            return speaker;
        }
        [PamelloCommand]
        public async Task SpeakerDisconnect(IPamelloSpeaker speaker)
        {
            _speakers.Delete(speaker);
        }
        [PamelloCommand]
        public async Task<string> SpeakerInternetRename(IPamelloInternetSpeaker speaker, string newName) {
            return speaker.Name = newName;
        }
    }
}
