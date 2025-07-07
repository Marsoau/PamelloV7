using PamelloV7.Core.Model.Entities;

namespace PamelloV7.Core.Model.Audio
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
