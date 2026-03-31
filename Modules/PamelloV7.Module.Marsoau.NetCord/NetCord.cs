using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using PamelloV7.Audio.Modules;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Modules;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;
using PamelloV7.Module.Marsoau.Base.Entities;
using PamelloV7.Module.Marsoau.NetCord.Config;
using PamelloV7.Module.Marsoau.NetCord.Logger;

namespace PamelloV7.Module.Marsoau.NetCord;

public class NetCord : IPamelloModule
{
    public string Name => "NetCord";
    public string Author => "Marsoau";
    public string Description => "NetCord test with DAVE";
    public ELoadingStage Stage => ELoadingStage.Default;
    public IBrush Color => Brushes.MediumTurquoise;

    public async Task StartupAsync(IServiceProvider services) {
        Output.Write("NetCord setup");

        const ulong guildId = 1304142495453548646;
        const ulong vcId = 1304142495453548650;
        
        var clientReady = new TaskCompletionSource();
        
        var client = new GatewayClient(new BotToken(NetCordConfig.Root.Token), new GatewayClientConfiguration {
            Logger = new NetCordLogger(),
        });
        
        client.Ready += async _ => clientReady.SetResult();
        
        await client.StartAsync();
        await clientReady.Task;
        
        Output.Write("NetCord started, creating voice playback");
        
        var voiceClient = await client.JoinVoiceChannelAsync(guildId, vcId);
        await voiceClient.StartAsync();
        
        var voiceStream = voiceClient.CreateVoiceStream();
        var pcmStream = new OpusEncodeStream(voiceStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);
        
        await voiceClient.EnterSpeakingStateAsync(new SpeakingProperties(SpeakingFlags.Microphone));
        
        var audio = services.GetRequiredService<IPamelloAudioSystem>();
        var songs = services.GetRequiredService<IPamelloSongRepository>();
        
        var song = audio.RegisterModule(new SongAudio(songs.GetRequired(139), services));
        var pump = audio.RegisterModule(new AudioPump(3840));

        await song.TryInitialize();
        
        pump.Input.ConnectedPoint = song.Output;
        pump.Output.ProcessAudio += (bytes, wait, t) => {
            pcmStream.Write(bytes);
            return true;
        };
        
        _ = pump.Start();
        
        Output.Write("Fully started NetCord");
    }
}
