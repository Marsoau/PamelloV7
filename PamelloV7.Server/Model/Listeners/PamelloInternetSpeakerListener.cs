

using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Points;

namespace PamelloV7.Server.Model.Listeners
{
    public class PamelloInternetSpeakerListener : PamelloListener, IAudioModuleWithInputs<AudioPushPoint>
    {
        public int MinInputs => 1;
        public int MaxInputs => 1;

        public AudioPushPoint Input;
        
        public readonly PamelloUser? User;
        
        public PamelloInternetSpeakerListener(HttpResponse response, PamelloUser? user) : base(response) {
            User = user;
        }

        public override async Task InitializeConnecion() {
            _response.ContentType = "audio/mpeg";
            _response.Headers.CacheControl = "no-cache";
            await _response.Body.FlushAsync();
        }

        public async Task SendAudio(byte[] audio) {
            try {
                if (!IsClosed) {
                    await _response.Body.WriteAsync(audio);
                    await _response.Body.FlushAsync();
                    Console.WriteLine($"Sent {audio.Length} bytes of audio to listener");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error sending audio: {ex.Message}");
                await CloseConnection();
            }
        }
        
        protected override async Task CloseConnectionBase() {
            IsClosed = true;
            try {
                await _response.CompleteAsync();
            }
            catch (ObjectDisposedException x) {
                // Already disposed, ignore
            }
        }

        public AudioPushPoint CreateInput() {
            Input = new AudioPushPoint();

            Input.Process = SendAudio;
            
            return Input;
        }

        public void InitModule() {
        }
    }
}
