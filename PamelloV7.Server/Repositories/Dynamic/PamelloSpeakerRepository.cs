using Discord.WebSocket;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Audio;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Server.Model;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Repositories.Dynamic;

public class PamelloSpeakerRepository : IPamelloSpeakerRepository, IDisposable
{
    private readonly IServiceProvider _services;
    
    private DiscordClientService _discordClients;
    private IPamelloPlayerRepository _players;

    private List<IPamelloSpeaker> _speakers;

    public PamelloSpeakerRepository(IServiceProvider services) {
        _services = services;

        _speakers = [];
    }

    public event Action? BeforeLoading;
    public event Action<int, int>? OnLoadingProgress;
    public event Action? OnLoaded;
    public event Action? BeforeInit;
    public event Action<int, int>? OnInitProgress;
    public event Action? OnInit;

    public void InitServices() {
        _discordClients = _services.GetRequiredService<DiscordClientService>();
        _players = _services.GetRequiredService<IPamelloPlayerRepository>();
    }

    public Task LoadAllAsync() {
        throw new NotImplementedException();
    }

    public Task InitAllAsync() {
        throw new NotImplementedException();
    }

    private IEnumerable<TSpeaker> GetSpeakers<TSpeaker>()
        where TSpeaker : IPamelloSpeaker
        => _speakers.OfType<TSpeaker>();

    public IPamelloSpeaker GetRequired(int id)
        => GetRequired<IPamelloSpeaker>(id);
    public TSpeaker GetRequired<TSpeaker>(int id)
        where TSpeaker : IPamelloSpeaker
        => Get<TSpeaker>(id) ?? throw new PamelloException($"Speaker with id \"{id}\" was not found");
    public IPamelloSpeaker? Get(int id) {
        return Get<IPamelloSpeaker>(id);
    }
    public TSpeaker? Get<TSpeaker>(int id)
        where TSpeaker : IPamelloSpeaker
    {
        return GetSpeakers<TSpeaker>().FirstOrDefault(speaker => speaker.Id == id);
    }

    public async Task<IPamelloSpeaker> GetByValueRequired(string value, IPamelloUser? scopeUser)
        => await GetByValueRequired<IPamelloSpeaker>(value, scopeUser);
    public async Task<TSpeaker> GetByValueRequired<TSpeaker>(string value, IPamelloUser? scopeUser)
        where TSpeaker : class, IPamelloSpeaker
        => await GetByValue<TSpeaker>(value, scopeUser) ?? throw new PamelloException($"Speaker with value \"{value}\" was not found");
    public Task<IPamelloSpeaker?> GetByValue(string value, IPamelloUser? scopeUser) {
        return GetByValue<IPamelloSpeaker>(value, scopeUser);
    }
    public async Task<TSpeaker?> GetByValue<TSpeaker>(string value, IPamelloUser? scopeUser)
        where TSpeaker : class, IPamelloSpeaker {
        var scopeSpeakers = _speakers.OfType<TSpeaker>();
        
        TSpeaker? speaker = null;

        if (int.TryParse(value, out var id)) {
            if (scopeUser is not null)
            {
                speaker = scopeSpeakers.FirstOrDefault(s => s.Id == id);
            }
        }
        /*
        else if (value == "current") {
            var userVc = _discordClients.GetUserVoiceChannel(scopeUser);
            if (userVc is null) return null;
            
            speaker = scopeSpeakers.OfType<PamelloDiscordSpeaker>().FirstOrDefault(s => s.Voice.Id == userVc.Id) as TSpeaker;
        }
        */
        else {
            speaker = scopeSpeakers.OfType<IPamelloInternetSpeaker>().FirstOrDefault(s => s.Name == value) as TSpeaker;
        }

        return speaker;
    }

    public Task<IEnumerable<IPamelloSpeaker>> SearchAsync(string querry, IPamelloUser? scopeUser = null)
        => SearchAsync<IPamelloSpeaker>(querry, scopeUser);
    public Task<IEnumerable<TSpeaker>> SearchAsync<TSpeaker>(string querry, IPamelloUser? scopeUser = null)
        where TSpeaker : IPamelloSpeaker
        => Task.Run(() => Search<TSpeaker>(querry, scopeUser));
    public IEnumerable<IPamelloSpeaker> Search(string querry, IPamelloUser? scopeUser = null)
        => Search<IPamelloSpeaker>(querry, scopeUser);
    public IEnumerable<TSpeaker> Search<TSpeaker>(string querry, IPamelloUser? scopeUser = null)
        where TSpeaker : IPamelloSpeaker
    {
        if (scopeUser is null) return [];
        
        var scopeSpeakers = new HashSet<IPamelloSpeaker>();

        if (scopeUser.SelectedPlayer is not null)
        {
            foreach (var speaker in _speakers) {
                if (((PamelloSpeaker)speaker).Player == scopeUser.SelectedPlayer) scopeSpeakers.Add(speaker);
            }
        }

        var results = scopeSpeakers.OfType<TSpeaker>();
        if (querry?.Length > 0) {
            results = results.Where(speaker =>
                speaker.Name.Contains(querry, StringComparison.CurrentCultureIgnoreCase));
        }
        
        return results;
    }
    
    public List<IPamelloPlayer> GetVoicePlayers(ulong vcId) {
        return [];
        /*
        var players = new HashSet<IPamelloPlayer>();

        var speakers = GetSpeakers<PamelloDiscordSpeaker>();
        foreach (var discordSpeaker in speakers) {
            if (discordSpeaker.Voice.Id == vcId) {
                players.Add(discordSpeaker.Player);
            }
        }

        return players.ToList();
        */
    }
    
    /* DISCORD SPEAKERS
    public async Task<IPamelloDiscordSpeaker> ConnectDiscord(IPamelloPlayer player, ulong guildId, ulong vcId) {
        PamelloDiscordSpeaker? newSpeaker;

        SocketGuildUser? speakerUser;
        foreach (var speakerClient in _discordClients.DiscordClients) {
            speakerUser = speakerClient.GetGuild(guildId)?.GetUser(speakerClient.CurrentUser.Id);
            if (speakerUser is null) continue;

            if (speakerUser.VoiceChannel is not null) {
                continue;
            }
            
            if ((newSpeaker = await player.AddDiscord(speakerClient, guildId, vcId)) is null) break;
            
            _speakers.Add(newSpeaker);
            return newSpeaker;
        }

        throw new PamelloException("No available speakers left");
    }
    */
    
    public async Task<IPamelloInternetSpeaker> ConnectInternet(IPamelloPlayer player, string? name) {
        if (name is not null) {
            if (!IsInternetSpeakerNameAvailable(name)) {
                throw new Exception($"Speaker name {name} is unavailable");
            }
        }

        var speaker = await player.AddInternet(name);
        _speakers.Add(speaker);

        return speaker;
    }
    
    public bool IsInternetSpeakerNameAvailable(string name) {
        var speakers = GetSpeakers<IPamelloInternetSpeaker>();
        return speakers.All(internetSpeaker => internetSpeaker.Name != name);
    }

    public void Delete(IPamelloSpeaker speaker) {
        ((PamelloSpeaker)speaker).Dispose();
        _speakers.Remove(speaker);
    }

    public void Dispose() {
        foreach (var speaker in _speakers) {
            ((PamelloSpeaker)speaker).Dispose();
        }
    }
}
