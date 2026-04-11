namespace PamelloV7.Module.Marsoau.NetCord.Differentiation;

public record Differentiator(
    ulong InteractionId,
    int FollowUpIndex,
    int PartIndex
) {
    public override string ToString() => $"{InteractionId}-{FollowUpIndex}-{PartIndex}";
}
