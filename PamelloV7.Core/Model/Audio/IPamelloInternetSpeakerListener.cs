using PamelloV7.Core.Model.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace PamelloV7.Core.Model.Audio
{
    public interface IPamelloInternetSpeakerListener : IPamelloListener
    {
        public IPamelloUser? User { get; }
        
        public Task InitializeConnection();
    }
}
