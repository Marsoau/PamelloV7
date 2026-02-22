namespace PamelloV7.Framework.DTO.Speakers;

public record PamelloInternetSpeakerDTO : PamelloSpeakerDTO
{
    public int ListenersCount { get; set; }
}