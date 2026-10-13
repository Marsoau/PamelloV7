using System.Threading.Channels;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;

namespace PamelloV7.Server.Audio;

public partial class InternetSpeakerSink : AudioModule, IAudioModuleWithInput
{
    private readonly HttpResponse _response;

    private readonly Channel<byte[]> _queue = Channel.CreateBounded<byte[]>(
        new BoundedChannelOptions(256) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true });

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
        if (!IsInitialized || RequestAbortedToken.IsCancellationRequested) return false;
        _queue.Writer.TryWrite(audio);
        return true;
    }

    public async Task RunAsync() {
        try {
            await foreach (var chunk in _queue.Reader.ReadAllAsync(RequestAbortedToken)) {
                await _response.Body.WriteAsync(chunk, RequestAbortedToken);
                await _response.Body.FlushAsync(RequestAbortedToken);
            }
        }
        catch (OperationCanceledException) { }
        catch (IOException) { }
    }

    protected override void Dispose(bool isDisposing) {
        _queue.Writer.TryComplete();
        base.Dispose(isDisposing);
    }
}
