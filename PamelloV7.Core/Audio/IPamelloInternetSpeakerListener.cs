using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Audio
{
    public interface IPamelloInternetSpeakerListener : IPamelloListener
    {
        public IPamelloUser? User { get; }
        
        public Task InitializeConnection();
    }
}
