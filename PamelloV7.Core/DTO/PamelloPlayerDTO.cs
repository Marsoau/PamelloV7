using PamelloV7.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloPlayerDTO : IPamelloDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }


        [JsonPropertyName("isPaused")]
        public bool IsPaused { get; set; }

        [JsonPropertyName("state")]
        public EPlayerState State { get; set; }

        [JsonPropertyName("isProtected")]
        public bool IsProtected { get; set; }


        [JsonPropertyName("currentSongId")]
        public int? CurrentSongId { get; set; }

        [JsonPropertyName("queueEntriesDTOs")]
        public IEnumerable<PamelloQueueEntryDTO> QueueEntriesDTOs { get; set; }

        [JsonPropertyName("queuePosition")]
        public int QueuePosition { get; set; }

        [JsonPropertyName("currentEpisodePosition")]
        public int? CurrentEpisodePosition { get; set; }

        [JsonPropertyName("newPositionRequest")]
        public int? NextPositionRequest { get; set; }


        [JsonPropertyName("currentSongTinePassed")]
        public int CurrentSongTimePassed { get; set; }

        [JsonPropertyName("currentSongTimeTotal")]
        public int CurrentSongTimeTotal { get; set; }


        [JsonPropertyName("queueIsRandom")]
        public bool QueueIsRandom { get; set; }

        [JsonPropertyName("queueIsReverser")]
        public bool QueueIsReversed { get; set; }

        [JsonPropertyName("queueIsNoLeftovers")]
        public bool QueueIsNoLeftovers { get; set; }

        [JsonPropertyName("queueIsFeedRandom")]
        public bool QueueIsFeedRandom { get; set; }
    }
}
