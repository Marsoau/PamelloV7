using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Entities;
using PamelloV7.Server.Model.AudioOld;
using PamelloV7.Server.Model.AudioOld.Interfaces;
using PamelloV7.Server.Model.AudioOld.Points;

namespace PamelloV7.Server.Model.Listeners
{
    public class PamelloInternetSpeakerListener : PamelloListener, IPamelloInternetSpeakerListener, IAudioModuleWithInputs<AudioPushPoint>
    {
        public int MinInputs => 1;
        public int MaxInputs => 1;

        public AudioModel ParentModel { get; }

        public AudioPushPoint Input;

        public IPamelloUser? User { get; }

        public CancellationToken Cancellation { get; }
        public bool IsDisposed { get; private set; }
        
        public PamelloInternetSpeakerListener(
            AudioModel parentModel,
            HttpResponse response,
            CancellationToken cancellationToken,
            IPamelloUser? user
        ) : base(response) {
            ParentModel = parentModel;
            
            User = user;
            Cancellation = cancellationToken;

            Cancellation.Register(Dispose);
        }

        public override async Task InitializeConnection() {
            _response.ContentType = "audio/mpeg";
            _response.Headers.CacheControl = "no-cache";
            await _response.Body.FlushAsync(Cancellation);
        }

        private async Task<bool> SendAudio(byte[] audio, bool wait, CancellationToken token) {
            try {
                if (!IsClosed && !Cancellation.IsCancellationRequested) {
                    try {
                        await _response.Body.WriteAsync(audio, Cancellation);
                        await _response.Body.FlushAsync(Cancellation);
                    }
                    catch {
                        return false;
                    }
                    
                    return true;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error sending audio: {ex.Message}");
                await CloseConnection();
            }

            return false;
        }
        
        protected override async Task CloseConnectionBase() {
            IsClosed = true;
            try {
                await _response.CompleteAsync();
                Dispose();
            }
            catch (ObjectDisposedException x) {
                // Already disposed, ignore
            }
        }

        public AudioPushPoint CreateInput() {
            Input = new AudioPushPoint(this);

            Input.Process = SendAudio;
            
            return Input;
        }

        public void InitModule() {
        }

        public override void Dispose()
        {
            IsDisposed = true;
            Console.WriteLine("disposing listener");
            
            Input.Dispose();
        }
    }
}
