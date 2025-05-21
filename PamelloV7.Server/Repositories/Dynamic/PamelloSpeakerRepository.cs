using Discord.WebSocket;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories.Dynamic;

public class PamelloSpeakerRepository : IPamelloRepository<PamelloSpeaker>, IDisposable
{
    private readonly IServiceProvider _services;
    
    private DiscordClientService _discordClients;
    private PamelloPlayerRepository _players;

    private List<PamelloSpeaker> _speakers;

    public PamelloSpeakerRepository(IServiceProvider services) {
        _services = services;

        _speakers = [];
    }
    
    public void InitServices() {
        _discordClients = _services.GetRequiredService<DiscordClientService>();
        _players = _services.GetRequiredService<PamelloPlayerRepository>();
    }

    private IEnumerable<TSpeaker> GetSpeakers<TSpeaker>()
    where TSpeaker : PamelloSpeaker
        => _speakers.OfType<TSpeaker>();

    public PamelloSpeaker GetRequired(int id)
        => GetRequired<PamelloSpeaker>(id);
    public PamelloSpeaker GetRequired<TSpeaker>(int id)
        where TSpeaker : PamelloSpeaker
        => Get<TSpeaker>(id) ?? throw new PamelloException($"Speaker with id \"{id}\" was not found");
    public PamelloSpeaker? Get(int id) {
        return Get<PamelloSpeaker>(id);
    }
    public PamelloSpeaker? Get<TSpeaker>(int id)
        where TSpeaker : PamelloSpeaker
    {
        return GetSpeakers<TSpeaker>().FirstOrDefault(speaker => speaker.Id == id);
    }

    public async Task<PamelloSpeaker> GetByValueRequired(string value, PamelloUser? scopeUser)
        => await GetByValueRequired<PamelloSpeaker>(value, scopeUser);
    public async Task<TSpeaker> GetByValueRequired<TSpeaker>(string value, PamelloUser? scopeUser)
    where TSpeaker : PamelloSpeaker
        => await GetByValue<TSpeaker>(value, scopeUser) ?? throw new PamelloException($"Speaker with value \"{value}\" was not found");
    public Task<PamelloSpeaker?> GetByValue(string value, PamelloUser? scopeUser) {
        return GetByValue<PamelloSpeaker>(value, scopeUser);
    }
    public async Task<TSpeaker?> GetByValue<TSpeaker>(string value, PamelloUser? scopeUser)
        where TSpeaker : PamelloSpeaker {
        var scopeSpeakers = await Search<TSpeaker>("", scopeUser);
        
        TSpeaker? speaker = null;

        if (int.TryParse(value, out var id)) {
            if (scopeUser is not null) {
                speaker = scopeSpeakers.FirstOrDefault(s => s.Id == id);
            }
        }
        else {
            speaker = scopeSpeakers.OfType<PamelloInternetSpeaker>().FirstOrDefault(s => s.Channel == value) as TSpeaker;
        }

        return speaker;
    }

    public Task<IEnumerable<PamelloSpeaker>> Search(string querry, PamelloUser? scopeUser = null) {
        return Search<PamelloSpeaker>(querry, scopeUser);
    }
    public Task<IEnumerable<TSpeaker>> Search<TSpeaker>(string querry, PamelloUser? scopeUser = null)
        where TSpeaker : PamelloSpeaker {
        return Task.FromResult(_speakers.OfType<TSpeaker>());
        var scopeSpeakers = new HashSet<PamelloSpeaker>();
        
        foreach (var speaker in _speakers) {
            if (speaker is PamelloInternetSpeaker { IsPublic: false } internetSpeaker && !(internetSpeaker.Player == scopeUser?.SelectedPlayer || internetSpeaker.Player.Creator == scopeUser)) continue;
            
            scopeSpeakers.Add(speaker);
        }

        var results = scopeSpeakers.OfType<TSpeaker>();
        if (querry?.Length > 0) {
            results = results.Where(speaker =>
                speaker.Name.Contains(querry, StringComparison.CurrentCultureIgnoreCase));
        }
        
        return Task.FromResult(_speakers.OfType<TSpeaker>());
    }
    
    public List<PamelloPlayer> GetVoicePlayers(ulong vcId) {
        var players = new HashSet<PamelloPlayer>();

        var speakers = GetSpeakers<PamelloDiscordSpeaker>();
        foreach (var discordSpeaker in speakers) {
            if (discordSpeaker.Voice.Id == vcId) {
                players.Add(discordSpeaker.Player);
            }
        }

        return players.ToList();
    }
    
    public async Task<PamelloDiscordSpeaker> ConnectDiscord(PamelloPlayer player, ulong guildId, ulong vcId) {
        PamelloDiscordSpeaker? newSpeaker;

        SocketGuildUser speakerUser;
        foreach (var speakerClient in _discordClients.DiscordClients) {
            speakerUser = speakerClient.GetGuild(guildId).GetUser(speakerClient.CurrentUser.Id);
            if (speakerUser is null) continue;

            if (speakerUser.VoiceChannel is not null) {
                continue;
            }
            
            //if ((newSpeaker = await player.Speakers.AddDiscord(speakerClient, guildId, vcId)) is not null) return newSpeaker;
        }

        throw new PamelloException("No available speakers left");
    }
    
    public async Task<PamelloInternetSpeaker> ConnectInternet(PamelloPlayer player, string? channel = null, bool isPublic = false) {
        if (channel is null) {
            var channelN = 1;
            while (!IsInternetChannelAvailable($"c-{channelN++}"));
                
            channel = $"c-{channelN}";
        }
        else {
            if (!IsInternetChannelAvailable(channel)) {
                throw new Exception($"Channel {channel} is unavailable");
            }
        }

        var speaker = await player.AddInternet(channel, isPublic);
        
        _speakers.Add(speaker);

        return speaker;
    }
    
    public bool IsInternetChannelAvailable(string channel) {
        var speakers = GetSpeakers<PamelloInternetSpeaker>();
        return speakers.All(internetSpeaker => internetSpeaker.Channel != channel);
    }

    public void Dispose() {
        Console.WriteLine("Disposing speakers");
    }
}