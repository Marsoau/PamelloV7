//auto generated

namespace PamelloV7.Wrapper.Extensions;

public static class PamelloCommandsExtensions {
    public static Task HistoryRecordRevert(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string record) {
        return commands.Invoker.ExecuteCommandAsync($"HistoryRecordRevert?record={record}");
    }
    public static Task<System.Int32> PlayerCreate(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String name) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"PlayerCreate?name={name}");
    }
    public static Task PlayerDelete(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync("PlayerDelete");
    }
    public static Task<System.Boolean> PlayerPause(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>("PlayerPause");
    }
    public static Task<System.Boolean> PlayerPauseToggle(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>("PlayerPauseToggle");
    }
    public static Task<System.Boolean> PlayerProtectionSet(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.Boolean state) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"PlayerProtectionSet?state={state}");
    }
    public static Task PlayerQueueClear(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync("PlayerQueueClear");
    }
    public static Task PlayerQueueCurrentSongRewind(this PamelloV7.Wrapper.Commands.PamelloCommands commands, PamelloV7.Core.Audio.AudioTime time) {
        return commands.Invoker.ExecuteCommandAsync($"PlayerQueueCurrentSongRewind?time={time}");
    }
    public static Task<System.Int32> PlayerQueueGoTo(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String position, System.Boolean returnBack) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"PlayerQueueGoTo?position={position}&returnBack={returnBack}");
    }
    public static Task<System.Int32> PlayerQueueGoToEpisode(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String position) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"PlayerQueueGoToEpisode?position={position}");
    }
    public static Task<System.Boolean> PlayerQueueIsFeedRandomSet(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.Boolean state) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"PlayerQueueIsFeedRandomSet?state={state}");
    }
    public static Task<System.Boolean> PlayerQueueIsFeedRandomToggle(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>("PlayerQueueIsFeedRandomToggle");
    }
    public static Task<System.Boolean> PlayerQueueIsNoLeftoversSet(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.Boolean state) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"PlayerQueueIsNoLeftoversSet?state={state}");
    }
    public static Task<System.Boolean> PlayerQueueIsNoLeftoversToggle(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>("PlayerQueueIsNoLeftoversToggle");
    }
    public static Task<System.Boolean> PlayerQueueIsRandomSet(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.Boolean state) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"PlayerQueueIsRandomSet?state={state}");
    }
    public static Task<System.Boolean> PlayerQueueIsRandomToggle(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>("PlayerQueueIsRandomToggle");
    }
    public static Task<System.Boolean> PlayerQueueIsReversedSet(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.Boolean state) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"PlayerQueueIsReversedSet?state={state}");
    }
    public static Task<System.Boolean> PlayerQueueIsReversedToggle(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>("PlayerQueueIsReversedToggle");
    }
    public static Task<IEnumerable<System.Int32>> PlayerQueuePlaylistAdd(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string playlists, System.String position) {
        return commands.Invoker.ExecuteCommandAsync<IEnumerable<System.Int32>>($"PlayerQueuePlaylistAdd?playlists={playlists}&position={position}");
    }
    public static Task<System.Int32?> PlayerQueueRequestNextPosition(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String position) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32?>($"PlayerQueueRequestNextPosition?position={position}");
    }
    public static Task<System.Int32> PlayerQueueSkip(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>("PlayerQueueSkip");
    }
    public static Task<IEnumerable<System.Int32>> PlayerQueueSongAdd(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string songs) {
        return commands.Invoker.ExecuteCommandAsync<IEnumerable<System.Int32>>($"PlayerQueueSongAdd?songs={songs}");
    }
    public static Task PlayerQueueSongMove(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String fromPosition, System.String toPosition) {
        return commands.Invoker.ExecuteCommandAsync($"PlayerQueueSongMove?fromPosition={fromPosition}&toPosition={toPosition}");
    }
    public static Task<System.Int32> PlayerQueueSongRemove(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String position) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"PlayerQueueSongRemove?position={position}");
    }
    public static Task<System.Int32> PlayerQueueSongsRemoveRange(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String fromPosition, System.String toPosition) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"PlayerQueueSongsRemoveRange?fromPosition={fromPosition}&toPosition={toPosition}");
    }
    public static Task PlayerQueueSongSwap(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.String inPosition, System.String withPosition) {
        return commands.Invoker.ExecuteCommandAsync($"PlayerQueueSongSwap?inPosition={inPosition}&withPosition={withPosition}");
    }
    public static Task<System.Boolean> PlayerResume(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>("PlayerResume");
    }
    public static Task<System.Int32> PlayerSelect(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string player) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"PlayerSelect?player={player}");
    }
    public static Task<IEnumerable<System.Int32>> PlaylistFavoriteClear(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<IEnumerable<System.Int32>>("PlaylistFavoriteClear");
    }
    public static Task<System.Int32> PlaylistRename(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string playlist, System.String newName) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"PlaylistRename?playlist={playlist}&newName={newName}");
    }
    public static Task<System.Boolean> SongAssociationsAdd(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string song, System.String newAssociation) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"SongAssociationsAdd?song={song}&newAssociation={newAssociation}");
    }
    public static Task<System.Boolean> SongAssociationsRemove(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string song, System.String association) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"SongAssociationsRemove?song={song}&association={association}");
    }
    public static Task<IEnumerable<System.Int32>> SongFavoritesAdd(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string songs) {
        return commands.Invoker.ExecuteCommandAsync<IEnumerable<System.Int32>>($"SongFavoritesAdd?songs={songs}");
    }
    public static Task<IEnumerable<System.Int32>> SongFavoritesClear(this PamelloV7.Wrapper.Commands.PamelloCommands commands) {
        return commands.Invoker.ExecuteCommandAsync<IEnumerable<System.Int32>>("SongFavoritesClear");
    }
    public static Task<System.Int32> SongFavoritesMove(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.Int32 fromPosition, System.Int32 toPosition) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"SongFavoritesMove?fromPosition={fromPosition}&toPosition={toPosition}");
    }
    public static Task<IEnumerable<System.Int32>> SongFavoritesRemove(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string songs) {
        return commands.Invoker.ExecuteCommandAsync<IEnumerable<System.Int32>>($"SongFavoritesRemove?songs={songs}");
    }
    public static Task<System.Int32> SongInfoReset(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string song, System.Int32 index) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"SongInfoReset?song={song}&index={index}");
    }
    public static Task<System.Int32> SongRename(this PamelloV7.Wrapper.Commands.PamelloCommands commands, string song, System.String newName) {
        return commands.Invoker.ExecuteCommandAsync<System.Int32>($"SongRename?song={song}&newName={newName}");
    }
    public static Task<System.Boolean> UserAuthorizationSelect(this PamelloV7.Wrapper.Commands.PamelloCommands commands, System.Int32 index) {
        return commands.Invoker.ExecuteCommandAsync<System.Boolean>($"UserAuthorizationSelect?index={index}");
    }
}
