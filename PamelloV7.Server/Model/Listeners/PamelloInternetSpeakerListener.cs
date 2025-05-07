

namespace PamelloV7.Server.Model.Listeners
{
    public class PamelloInternetSpeakerListener : PamelloListener
    {
        public PamelloInternetSpeakerListener(HttpResponse response) : base(response) {
        }

        public override async Task InitializeConnecion() {
            _response.ContentType = "audio/mpeg";
            _response.Headers["Cache-Control"] = "no-cache";
            await _response.Body.FlushAsync();
        }

        public async Task SendAudio(byte[] audio) {
            try {
                await _response.Body.WriteAsync(audio);
                await _response.Body.FlushAsync();
            }
            catch {
                await CloseConnection();
            }
        }

        protected override async Task CloseConnectionBase() {
            IsClosed = true;
            try {
                await _response.CompleteAsync();
            }
            catch (ObjectDisposedException x) {

            }
        }
    }
}
