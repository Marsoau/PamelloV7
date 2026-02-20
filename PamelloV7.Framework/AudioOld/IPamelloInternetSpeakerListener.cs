using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.AudioOld
{
    public interface IPamelloInternetSpeakerListener : IPamelloListener
    {
        public IPamelloUser? User { get; }
        
        public Task InitializeConnection();
    }
}
