using PamelloV7.Core.DTO;
using PamelloV7.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.WrapperOld.Model
{
    public class RemotePlayer : RemoteEntity<PamelloPlayerDTO>
    {
        public RemotePlayer(PamelloPlayerDTO dto, PamelloClient client) : base(dto, client) {

        }

        public bool IsPaused {
            get => _dto.IsPaused;
        }
        public EPlayerState State {
            get => _dto.State;
        }

        public int? CurrentSongId {
            get => _dto.CurrentSongId;
        }
        public IEnumerable<PamelloQueueEntryDTO> QueueEntriesDTOs {
            get => _dto.QueueEntriesDTOs;
        }
        public int QueuePosition {
            get => _dto.QueuePosition;
        }
        public int? CurrentEpisodePosition {
            get => _dto.CurrentEpisodePosition;
        }
        public int? NextPositionRequest {
            get => _dto.NextPositionRequest;
        }

        public int CurrentSongTimePassed {
            get => _dto.CurrentSongTimePassed;
        }
        public int CurrentSongTimeTotal {
            get => _dto.CurrentSongTimeTotal;
        }

        public bool QueueIsRandom {
            get => _dto.QueueIsRandom;
        }
        public bool QueueIsReversed {
            get => _dto.QueueIsReversed;
        }
        public bool QueueIsNoLeftovers {
            get => _dto.QueueIsNoLeftovers;
        }
        public bool QueueIsFeedRandom {
            get => _dto.QueueIsFeedRandom;
        }

        public bool IsProtected {
            get => _dto.IsProtected;
        }

        public int OwnerId {
            get => _dto.OwnerId;
        }

        internal override void FullUpdate(PamelloPlayerDTO dto) {
            _dto = dto;
        }
    }
}
