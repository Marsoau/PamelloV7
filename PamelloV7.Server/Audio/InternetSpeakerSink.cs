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
        
        await _response.Body.FlushAsync(RequestAbortedToken);
    }
    
    private bool ProcessAudio(byte[] audio, bool wait, CancellationToken token) {
        Debug.WriteLine($"IsInitialized: {IsInitialized}");
        Debug.WriteLine($"token.IsCancellationRequested: {token.IsCancellationRequested}");
        Debug.WriteLine($"RequestAbortedToken.IsCancellationRequested: {RequestAbortedToken.IsCancellationRequested}");
        
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, RequestAbortedToken).Token;
        
        if (!IsInitialized || linkedToken.IsCancellationRequested) return false;

        try {
            _response.Body.WriteAsync(audio, 0, audio.Length, linkedToken).Wait(linkedToken);
            _response.Body.FlushAsync(linkedToken).Wait(linkedToken);
        }
        catch (OperationCanceledException) {
            Framework.Logging.Output.Write("ISL canceled");
            return false;
        }
        catch (Exception x) {
            Framework.Logging.Output.Write($"Exception in internat listener: {x}");
            return false;
        }
        
        return true;
    }

    protected override void Dispose(bool isDisposing) {
        base.Dispose(isDisposing);
    }
}
