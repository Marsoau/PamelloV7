using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Audio
{
    public interface IPamelloInternetSpeaker : IPamelloSpeaker
    {
        public int ListenersCount { get; }
        public Task<IPamelloInternetSpeakerListener> AddListener(
            object responseObject,
            CancellationToken cancellationToken,
            IPamelloUser? user
        );
    }
}
