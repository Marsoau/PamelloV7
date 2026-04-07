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
using PamelloV7.Module.Marsoau.NetCord.Audio;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Speakers;

public class PamelloDiscordSpeaker : PamelloDynamicEntity, IPamelloSpeaker, IAudioDependant
{
    private readonly ulong _guildId;
    
    public DiscordClientService _clients;
    
    private GatewayClient? _client;
    public GatewayClient Client => _client ?? throw new PamelloException("Client not registered");

    public override string Name => _client?.Cache.User?.GlobalName ?? $"Speaker-{Id}N";
    public override string SetName(string name, IPamelloUser scopeUser) {
        return Name;
    }
    
    public override bool IsDeleted { get; set; }
    
    public IPamelloPlayer Player { get; }
    public IEnumerable<IPamelloListener> Listeners { get; } = [];
    
    public AudioBuffer Buffer { get; }
    public AudioPump Pump { get; }
    public DiscordSpeakerSink Sink { get; }
    
    IAudioModuleWithInput IPamelloSpeaker.InputModule => Buffer;

    public PamelloDiscordSpeaker(int id, ulong guildId, IPamelloPlayer player, IServiceProvider services)
        : base(id, services) {
        _guildId = guildId;
        
        Player = player;
        
        _clients = services.GetRequiredService<DiscordClientService>();
        var audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Buffer = audio.RegisterModule(new AudioBuffer(3840 * 20));
        Pump = audio.RegisterModule(new AudioPump(3840));
        Sink = audio.RegisterModule(new DiscordSpeakerSink());
    }

    public void InitDependant() {
        Pump.Input.ConnectedPoint = Buffer.Output;
        Pump.Output.ConnectedPoint = Sink.Input;

        Pump.WaitOnInput = false;
        Pump.OnNoInput = () => {
            Sink.StopOpusAsync().Wait();
        };
        
        Pump.Start();
        
        Player.AddSpeaker(this);
    }

    public async Task ConnectAsync(ulong vcId) {
        _client = _clients.GetAvailableClient(_guildId);
        if (_client is null) throw new PamelloException("No available client found");
        
        var voiceClient = await Client.JoinVoiceChannelAsync(_guildId, vcId);
        await voiceClient.StartAsync();
        
        Sink.VoiceClient = voiceClient;
    }
    
    public bool IsAvailableFor(IPamelloUser user) {
        return true;
    }
}
