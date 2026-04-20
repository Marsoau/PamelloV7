using Microsoft.Extensions.DependencyInjection;
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
    
    private VoiceClient? VoiceClient { get; set; }
    
    public GatewayClient Client { get; }

    public override string Name => Client.Cache.User?.GlobalName ?? $"Speaker-{Id}N";
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

    public PamelloDiscordSpeaker(int id, GatewayClient client, IPamelloPlayer player, IServiceProvider services)
        : base(id, services) {
        _users = services.GetRequiredService<IPamelloUserRepository>();

        _listeners = [];
        
        
        Player = player;
        
        Client = client;
        
        Client.VoiceStateUpdate += ClientOnVoiceStateUpdate;
        
        var audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Buffer = audio.RegisterModule(new AudioBuffer(3840 * 20));
        Pump = audio.RegisterModule(new AudioPump(3840));
        Sink = audio.RegisterModule(new DiscordSpeakerSink());
    }

    private async ValueTask ClientOnVoiceStateUpdate(VoiceState state) {
        if (state.UserId == Client.Cache.User?.Id) {
            if (!state.ChannelId.HasValue) {
                Output.Write("Delete this speaker");
                return;
            }

            return;

            await Client.UpdateVoiceStateAsync(new VoiceStateProperties(VoiceClient.GuildId, null));
            VoiceClient = await Client.JoinVoiceChannelAsync(state.GuildId, state.ChannelId.Value);
            
            VoiceClient.Connect += async () => Output.Write("Connect");
            VoiceClient.Disconnect += async _ => Output.Write("Disconnect");
            VoiceClient.Connecting += async () => Output.Write("Connecting");
            
            await VoiceClient.StartAsync();
            
            return;
        }
        
        if (state.ChannelId.HasValue) {
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
        VoiceClient?.Disconnect -= VoiceClientOnDisconnect;
        
        await Client.UpdateVoiceStateAsync(new VoiceStateProperties(guildId, null));
        VoiceClient = await Client.JoinVoiceChannelAsync(guildId, vcId);
        
        await VoiceClient.StartAsync();
        
        VoiceClient.Connect += async () => Output.Write("Connect");
        VoiceClient.Disconnect += VoiceClientOnDisconnect;
        VoiceClient.Connecting += async () => Output.Write("Connecting");
        
        Sink.VoiceClient = VoiceClient;
    }

    private async ValueTask VoiceClientOnDisconnect(DisconnectEventArgs args) {
        Output.Write($"Disconnect: {args.Reconnect}");

        Sink.OpusStream = null;
        Sink.VoiceClient = null;

        await ConnectAsync(1463545154894823648, 1495774904065458308);
    }

    public void InitDependant() {
        Sink.VoiceClient = VoiceClient;
        
        Pump.Input.ConnectedPoint = Buffer.Output;
        Pump.Output.ConnectedPoint = Sink.Input;

        Pump.Condition = () => {
            if (VoiceClient.Status == WebSocketStatus.Ready) return true;
            
            Output.Write($"Condition: {VoiceClient.Status}");
            
            return false;
        };

        Pump.WaitOnInput = false;
        Pump.OnNoInput = () => {
            Sink.StopOpusAsync().Wait();
        };
        
        Pump.Start();
        
        Player.AddSpeaker(this);
    }
    
    public bool IsAvailableFor(IPamelloUser user) {
        return true;
    }
}
