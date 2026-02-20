using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.AudioOld
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
