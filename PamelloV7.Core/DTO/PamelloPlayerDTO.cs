using PamelloV7.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloPlayerDTO : IPamelloDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsPaused { get; set; }
        public EPlayerStatus State { get; set; }

        public int? CurrentSongId { get; set; }
        public IEnumerable<int> QueueSongsIds { get; set; }
        public int QueuePosition { get; set; }
        public int? CurrentEpisodePosition { get; set; }
        public int? NextPositionRequest { get; set; }

        public int CurrentSongTimePassed { get; set; }
        public int CurrentSongTimeTotal { get; set; }

        public bool QueueIsRandom { get; set; }
        public bool QueueIsReversed { get; set; }
        public bool QueueIsNoLeftovers { get; set; }
        public bool QueueIsFeedRandom { get; set; }
    }
}
