using System.Diagnostics.CodeAnalysis;
using Discord.Audio;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Attributes;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.Discord.Audio;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Speakers;

public class PamelloDiscordSpeaker : PamelloDynamicEntity, IPamelloSpeaker, IAudioDependant
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
    
    IAudioModuleWithInput IPamelloSpeaker.InputModule => Buffer;
    
    public bool IsAvailableFor(IPamelloUser user) {
        return Listeners.Any(listener => listener.User == user);
    }

    public IEnumerable<IPamelloListener> Listeners {
        get {
            if (Guild?.AudioClient is null) return [];
            var vc = Guild.GetUser(Client.CurrentUser.Id)?.VoiceChannel;
            
            if (vc is null) return [];

            return vc.ConnectedUsers.Where(user => user.Id != Client.CurrentUser.Id).Select(user =>
                new PamelloDiscordSpeakerListener(user, this, _services)
            );
        }
    }

    public override string Name => Client.CurrentUser.Username ?? $"Speaker-{Id}N";
    public override string SetName(string name, IPamelloUser scopeUser) {
        throw new PamelloException("Cannot set name of a discord speaker");
    }

    public override bool IsDeleted { get; set; }

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

        Output.Write($"Before connect: {Guild.AudioClient}");
        
        await vc.ConnectAsync();
    }

    private void Register(DiscordSocketClient client, SocketGuild guild) {
        Client = client;
        Guild = guild;
        
        Client.VoiceServerUpdated += ClientOnVoiceServerUpdated;
    }

    private async Task ClientOnVoiceServerUpdated(SocketVoiceServer voiceServer) {
        Output.Write($"VSU: {Guild.AudioClient}");
        Guild.AudioClient.Connected += async () => {
            Output.Write("AC Connected");
            Sink.Stream = Guild.AudioClient.CreatePCMStream(AudioApplication.Music);

            foreach (var listener in Listeners) {
                if (listener.User is null || listener.User.SelectedPlayer is not null) continue;
                
                listener.User.SelectPlayer(Player, true);
            }
        };
    }
}
