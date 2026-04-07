using NetCord.Gateway.Voice;
using PamelloV7.Framework.Audio.Modules.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Audio;

public partial class DiscordSpeakerSink : AudioModule, IAudioModuleWithInput
{
    public VoiceClient? VoiceClient { get; set; }
    
    public OpusEncodeStream? OpusStream { get; set; }

    protected override void InitAudioInternal(IServiceProvider services) {
        Input.ProcessAudio = ProcessAudio;
    }

    public async Task StartOpusAsync() {
        if (OpusStream is not null) return;
        if (VoiceClient is null) return;
        
        var voiceStream = VoiceClient.CreateVoiceStream();
        OpusStream = new OpusEncodeStream(voiceStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);
        
        await VoiceClient.EnterSpeakingStateAsync(new SpeakingProperties(SpeakingFlags.Microphone));
    }
    public async Task StopOpusAsync() {
        if (OpusStream is null) return;
        
        OpusStream.Flush();
        OpusStream.Dispose();
        OpusStream = null;
        
        if (VoiceClient is null) return;
        
        await VoiceClient.EnterSpeakingStateAsync(new SpeakingProperties(0));
    }

    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        StartOpusAsync().Wait(token);
        
        if (OpusStream is null) return false;
        
        OpusStream.Write(audio, 0, audio.Length);
        return true;
    }
}
