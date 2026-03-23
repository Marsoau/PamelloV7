using System.Diagnostics;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Framework.Logging;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Audio;

public partial class InternetSpeakerSink : AudioModule, IAudioModuleWithInput
{
    private readonly HttpResponse _response;
    
    public bool IsInitialized { get; set; }
    
    public CancellationToken RequestAbortedToken { get; set; }
    
    public InternetSpeakerSink(HttpResponse response, CancellationToken requestAbortedToken) {
        _response = response;
        
        IsInitialized = false;
        RequestAbortedToken = requestAbortedToken;
    }

    protected override void InitAudioInternal(IServiceProvider services) {
        Input.ProcessAudio = ProcessAudio;
    }

    public async Task InitializeConnection() {
        IsInitialized = true;
        
        _response.ContentType = "audio/mpeg";
        _response.Headers.CacheControl = "no-cache";
        
        await _response.Body.FlushAsync();
    }
    
    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        Debug.WriteLine($"IsInitialized: {IsInitialized}");
        Debug.WriteLine($"token.IsCancellationRequested: {token.IsCancellationRequested}");
        Debug.WriteLine($"RequestAbortedToken.IsCancellationRequested: {RequestAbortedToken.IsCancellationRequested}");
        
        if (
            !IsInitialized ||
            token.IsCancellationRequested ||
            RequestAbortedToken.IsCancellationRequested
        ) return false;
        
        try {
            _response.Body.WriteAsync(audio, 0, audio.Length, token).Wait(token);
            _response.Body.FlushAsync(token).Wait(token);
        }
        catch (Exception x) {
            Framework.Logging.Output.Write(x);
            return false;
        }
        
        return true;
    }
}
