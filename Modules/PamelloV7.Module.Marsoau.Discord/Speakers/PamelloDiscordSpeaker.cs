using System.Diagnostics.CodeAnalysis;
using Discord.Audio;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Discord.Audio;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Speakers;

public class PamelloDiscordSpeaker : PamelloEntity, IPamelloSpeaker, IAudioDependant
{
    private readonly DiscordClientService _clients;
    
    private readonly ulong _guildId;
    
    public DiscordSocketClient Client { get; private set; }
    public SocketGuild Guild { get; private set; }
    
    [OnAudioMap]
    public IPamelloPlayer Player { get; }
    
    public AudioBuffer Buffer { get; }
    public AudioPump Pump { get; }
    public SpeakerAudioSink Sink { get; }
    
    IAudioModule IPamelloSpeaker.Input => Buffer;

    public IEnumerable<IPamelloListener> Listeners {
        get {
            if (Guild?.AudioClient is null) return [];
            var vc = Guild.GetUser(Client.CurrentUser.Id)?.VoiceChannel;
            if (vc is null) return [];

            return vc.Users.Select(user =>
                new PamelloDiscordSpeakerListener(user, this, _services)
            );
        }
    }

    public override string Name {
        get => Client.CurrentUser.Username ?? $"Speaker-{Id}N";
        set => throw new PamelloException("Cannot set name of a discord speaker");
    }
    
    public PamelloDiscordSpeaker(int id, ulong guildId, IPamelloPlayer player, IServiceProvider services) : base(id, services) {
        _clients = services.GetRequiredService<DiscordClientService>();
        
        _guildId = guildId;
        
        Player = player;
        
        var audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Buffer = audio.RegisterModule(new AudioBuffer(102400));
        Pump = audio.RegisterModule(new AudioPump(128));
        Sink = audio.RegisterModule(new SpeakerAudioSink());
    }

    public void InitDependant() {
        Pump.Input.ConnectedPoint = Buffer.Output;
        Pump.Output.ConnectedPoint = Sink.Input;
        
        Pump.Start();
        
        Player.AddSpeaker(this);
    }

    public async Task ConnectAsync(ulong vcId) {
        var client = _clients.GetAvailableClient(_guildId);
        if (client is null) throw new PamelloException("No available client found");
        
        var guild = client.GetGuild(_guildId);
        if (guild is null) throw new PamelloException("Guild not found");
        
        var vc = guild.GetVoiceChannel(vcId);
        if (vc is null) throw new PamelloException("Voice channel not found");
        
        Register(client, guild);

        Console.WriteLine($"Before connect: {Guild.AudioClient}");
        
        await vc.ConnectAsync();
    }

    private void Register(DiscordSocketClient client, SocketGuild guild) {
        Client = client;
        Guild = guild;
        
        Client.VoiceServerUpdated += ClientOnVoiceServerUpdated;
    }

    private async Task ClientOnVoiceServerUpdated(SocketVoiceServer voiceServer) {
        Console.WriteLine($"VSU: {Guild.AudioClient}");
        Guild.AudioClient.Connected += async () => {
            Console.WriteLine("AC Connected");
            Sink.Stream = Guild.AudioClient.CreatePCMStream(AudioApplication.Music);

            foreach (var listener in Listeners) {
                if (listener.User is null || listener.User.SelectedPlayer is not null) continue;
                
                listener.User.SelectedPlayer = Player;
            }
        };
    }
}
