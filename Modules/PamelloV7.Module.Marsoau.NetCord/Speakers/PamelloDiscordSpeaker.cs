using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Audio;
using PamelloV7.Module.Marsoau.NetCord.Extensions;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Speakers;

public class PamelloDiscordSpeaker : PamelloDynamicEntity, IPamelloSpeaker, IAudioDependant
{
    private readonly IPamelloUserRepository _users;

    private readonly IPamelloSpeakerRepository _speakers;

    private readonly DiscordClientService _clients;
    
    private TaskCompletionSource _connectCompletion = new();
    private TaskCompletionSource<(ulong guildId, ulong vcId)> _disconnectCompletion = new();
   
    private VoiceClient? VoiceClient { get; set; }
    
    public GatewayClient? Client { get; private set; }

    public override string Name => Client?.Cache.User?.Username ?? $"Speaker-{Id}N";
    public override string SetName(string name, IPamelloUser scopeUser) {
        return Name;
    }
    
    public override bool IsDeleted { get; set; }
    
    public IPamelloPlayer Player { get; }

    private List<PamelloDiscordSpeakerListener> _listeners;
    public IEnumerable<IPamelloListener> Listeners => _listeners;
    
    public AudioBuffer Buffer { get; }
    public AudioPump Pump { get; }
    public DiscordSpeakerSink Sink { get; }
    
    IAudioModuleWithInput IPamelloSpeaker.InputModule => Buffer;

    public PamelloDiscordSpeaker(int id, IPamelloPlayer player, IServiceProvider services)
        : base(id, services) {
        _users = services.GetRequiredService<IPamelloUserRepository>();

        _speakers = services.GetRequiredService<IPamelloSpeakerRepository>();
        
        _clients = services.GetRequiredService<DiscordClientService>();

        _listeners = [];
        
        Player = player;

        var audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Buffer = audio.RegisterModule(new AudioBuffer(3840 * 20));
        Pump = audio.RegisterModule(new AudioPump(3840));
        Sink = audio.RegisterModule(new DiscordSpeakerSink());
    }

    private async ValueTask ClientOnVoiceStateUpdate(VoiceState state) {
        if (VoiceClient is null) return;
        
        if (state.UserId == VoiceClient.UserId && state.GuildId == VoiceClient.GuildId) {
            _disconnectCompletion.SetResult(state.ChannelId.HasValue
                ? (state.GuildId, state.ChannelId.Value)
                : (0, 0)
            );

            return;
        }

        if (state.ChannelId.HasValue && state.ChannelId == VoiceClient.ChannelId) {
            if (_listeners.Any(listener => listener.DiscordId == state.UserId))
                return;
            
            _listeners.Add(
                new PamelloDiscordSpeakerListener(this, state.UserId, _services)
            );
        }
        else {
            _listeners.RemoveAll(listener => listener.DiscordId == state.UserId);
        }
    }

    public async Task ConnectAsync(ulong guildId, ulong vcId) {
        Unsubscribe();
        
        ResetDisconnectCompletion();

        Client ??= _clients.GetAvailableClient(guildId) ?? throw new PamelloException($"No available client");
        
        await Client.UpdateVoiceStateAsync(new VoiceStateProperties(guildId, null));
        VoiceClient = await Client.JoinVoiceChannelAsync(guildId, vcId);
        
        Subscribe();
        
        await VoiceClient.StartAsync();

        Sink.VoiceClient = VoiceClient;
    }

    public void Unsubscribe() {
        Client?.VoiceStateUpdate -= ClientOnVoiceStateUpdate;
        
        VoiceClient?.Ready -= VoiceClientOnReady;
        VoiceClient?.Connect -= VoiceClientOnConnect;
        VoiceClient?.Connecting -= VoiceClientOnConnecting;
        VoiceClient?.Disconnect -= VoiceClientOnDisconnect;
    }

    public void Subscribe() {
        if (Client is null || VoiceClient is null) return;
        
        Client.VoiceStateUpdate += ClientOnVoiceStateUpdate;

        VoiceClient.Ready += VoiceClientOnReady;
        VoiceClient.Connect += VoiceClientOnConnect;
        VoiceClient.Disconnect += VoiceClientOnDisconnect;
        VoiceClient.Connecting += VoiceClientOnConnecting;
    }

    private ValueTask VoiceClientOnReady() {
        UpdateListeners();
        return ValueTask.CompletedTask;
    }

    private void UpdateListeners() {
        if (VoiceClient is null) return;

        _listeners = VoiceClient.Cache.Users.Select(userId => 
            new PamelloDiscordSpeakerListener(this, userId, _services)
        ).ToList();
        
        Output.Write("Voice Users:");
        foreach (var listener in _listeners) {
            Output.Write($"{listener.DiscordId}: {listener.User}");
        }
    }

    private void InitializeListenersAsync() {
        if (Client is null || VoiceClient is null) return;

        var guild = Client.Cache.Guilds.GetValueOrDefault(VoiceClient.GuildId);
        if (guild is null) return;

        var users = guild.VoiceStates.Values
            .Where(vs => vs.ChannelId == VoiceClient.ChannelId)
            .Where(vs => vs.UserId != VoiceClient.UserId)
            .Select(vs => vs.User)
            .OfType<GuildUser>();

        Output.Write("LisUsers");
        foreach (var user in users) {
            Output.Write(user.GlobalName);
        }
    }

    private async ValueTask VoiceClientOnConnect() {
        _connectCompletion.SetResult();
        _connectCompletion = new();
    }

    private int _connectingTimeout = 1;
    private int _disconnectTimeout = 1;

    private async ValueTask VoiceClientOnConnecting() {
        Output.Write("Connecting...");

        await Task.WhenAny(Task.Delay(_connectingTimeout * 1000), _connectCompletion.Task);
        
        if (!_connectCompletion.Task.IsCompleted) {
            Console.WriteLine("WASNT CONNECTED FOR IN A LONG TIME");
        }

        Output.Write($"Status aftrer: {VoiceClient?.Status}");
    }

    private async ValueTask VoiceClientOnDisconnect(DisconnectEventArgs args) {
        Output.Write($"Disconnect on: {VoiceClient?.SessionId}");

        Sink.OpusStream = null;
        Sink.VoiceClient = null;
        
        await Task.WhenAny(Task.Delay(_disconnectTimeout * 1000), _disconnectCompletion.Task);
        if (!_disconnectCompletion.Task.IsCompleted) {
            _disconnectCompletion.SetResult((1, 1));
        }

        var (guildId, channelId) = _disconnectCompletion.Task.Result;

        if (guildId == 0 || channelId == 0) {
            Output.Write("Deleted Should be");
            
            _speakers.Delete(this, null);
            
            return;
        }
        if (guildId == 1 || channelId == 1) {
            if (VoiceClient is null) return;
            
            guildId = VoiceClient.GuildId;
            channelId = VoiceClient.ChannelId;
        }
        
        await ConnectAsync(guildId, channelId);
    }

    private void ResetDisconnectCompletion() {
        if (!_disconnectCompletion.Task.IsCompleted) {
            _disconnectCompletion.SetResult((0, 0));
        }

        _disconnectCompletion = new();
    }

    public void InitDependant() {
        Sink.VoiceClient = VoiceClient;
        
        Pump.Input.ConnectedPoint = Buffer.Output;
        Pump.Output.ConnectedPoint = Sink.Input;

        Pump.Condition = () => {
            if (VoiceClient?.Status == WebSocketStatus.Ready) return true;
            
            Output.Write($"Condition: {VoiceClient?.Status}");

            return false;
        };

        Pump.WaitOnInput = false;
        Pump.OnNoInput = () => {
            Sink.StopOpusAsync().Wait();
        };
        
        Pump.Start();
        
        Player.AddSpeaker(this);
    }

    public void DeleteDependant() {
        Unsubscribe();
        
        Player.RemoveSpeaker(this);
    }

    public bool IsAvailableFor(IPamelloUser user) {
        return true;
    }
}
