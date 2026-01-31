using PamelloV7.Core.Entities;

namespace PamelloV7.Core.AudioOld
{
    public interface IPamelloInternetSpeakerListener : IPamelloListener
    {
        public IPamelloUser? User { get; }
        
        public Task InitializeConnection();
    }
}
