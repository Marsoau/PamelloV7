using PamelloV7.Framework.Audio.Attributes;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Server.Audio;

namespace PamelloV7.Server.Speakers;

public class PamelloInternetSpeakerListener : IPamelloListener, IAudioDependant
{
    public IPamelloUser? User { get; }
    public IPamelloSpeaker Speaker { get; }
    
    public InternetSpeakerSink Sink { get; set; }
    
    public TaskCompletionSource Lifetime { get; }
    

    public PamelloInternetSpeakerListener(HttpResponse response, CancellationToken requestAbortedToken, IPamelloSpeaker speaker, IPamelloUser? user, IServiceProvider services) {
        Speaker = speaker;
        User = user;
        
        Lifetime = new TaskCompletionSource();
        
        var audio = services.GetRequiredService<IPamelloAudioSystem>();
        
        Sink = audio.RegisterModule(new InternetSpeakerSink(response, requestAbortedToken));
    }
}
