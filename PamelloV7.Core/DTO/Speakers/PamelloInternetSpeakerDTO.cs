namespace PamelloV7.Core.DTO.Speakers;

public class PamelloInternetSpeakerDTO : PamelloSpeakerDTO
{
    public string Channel { get; set; }
    public bool IsPublic { get; set; }
    public int ListenersCount { get; set; }
}