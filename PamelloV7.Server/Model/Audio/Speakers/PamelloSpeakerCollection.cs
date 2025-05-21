using Discord.WebSocket;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server.Model.Audio.Speakers.Deleted;

public class PamelloSpeakerCollection : IDisposable
{
    private readonly IServiceProvider _services;

    private readonly PamelloSpeakerRepository _repository;
    
    private readonly PamelloPlayer _player;

    private readonly List<PamelloSpeaker> _speakers;
    
    private bool _isDisposed;

    public IReadOnlyList<PamelloSpeaker> All
        => _speakers;
    
    public PamelloSpeakerCollection(IServiceProvider services, PamelloPlayer player) {
        _services = services;
        
        _repository = services.GetRequiredService<PamelloSpeakerRepository>();
        
        _player = player;
        
        _speakers = [];
    }

    public async Task<PamelloDiscordSpeaker?> AddDiscord(DiscordSocketClient client, ulong guildId, ulong vcId) {
        var guild = client.GetGuild(guildId);
        if (guild is null) return null;

        var speaker = new PamelloDiscordSpeaker(_services, client, guild.Id, _player);
        await speaker.InitialConnect(vcId);

        _speakers.Add(speaker);
        speaker.OnTerminated += Speaker_Terminated;

        return speaker;
    }

    public async Task<PamelloInternetSpeaker> AddInternet(string channel, bool isPublic) {
        if (!_repository.IsInternetChannelAvailable(channel)) throw new PamelloException($"Channel \"{channel}\" is not available");
        
        var internetSpeaker = new PamelloInternetSpeaker(null, _player, channel, isPublic);
        //await internetSpeaker.InitialConnection();

        _speakers.Add(internetSpeaker);
        internetSpeaker.OnTerminated += Speaker_Terminated;

        return internetSpeaker;
    }

    private void Speaker_Terminated(PamelloSpeaker speaker) {
        _speakers.Remove(speaker);
    }

    public async Task BroadcastBytes(PamelloPlayer player, byte[] audio) {
        if (_isDisposed) return;
        
        foreach (var speaker in _speakers) {
            if (speaker.Player == player) {
                await speaker.PlayBytesAsync(audio);
            }
        }
    }

    public bool IsAnyAvailable() {
        return _speakers.Any(speaker => speaker.IsActive);
    }

    public void Dispose() {
        _isDisposed = true;
        
        var funcTasks = _speakers.Select(speaker => (Func<ValueTask>)speaker.DisposeAsync).ToList();
        var tasks = funcTasks.Select(func => func()).ToList();
    }
}