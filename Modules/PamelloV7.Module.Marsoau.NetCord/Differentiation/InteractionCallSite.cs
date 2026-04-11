using System.Collections.Frozen;
using System.Collections.Immutable;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Module.Marsoau.NetCord.Differentiation;

public record InteractionCallSite(
    Differentiator Differentiator,
    uint ClassHash,
    int Offset,
    int Key
) {
    public static string CustomIdPrefix => "tokenized:";
    
    public override string ToString() {
        var str = $"{Differentiator}_{ClassHash}-{Offset}";
        if (Key < 0) return str;
        
        return $"{str}_{Key}";
    }
    public string ToCustomId() => $"{CustomIdPrefix}{this}";
    
    public static InteractionCallSite FromString(string str) {
        var dcsParts = str.Replace(CustomIdPrefix, "").Split('_');
        
        var differentiatorParts = dcsParts.ElementAtOrDefault(0)?.Split('-');
        var hashOffsetParts = dcsParts.ElementAtOrDefault(1)?.Split('-');

        if (!int.TryParse(dcsParts.ElementAtOrDefault(2), out var key)) {
            key = -1;
        }
        
        if (differentiatorParts is null ||
            differentiatorParts.Length != 3 ||
            hashOffsetParts is null ||
            hashOffsetParts.Length != 2
        ) throw new PamelloException($"Invalid InteractionCallSite string: {str}");

        return new InteractionCallSite(
            new Differentiator(
                ulong.Parse(differentiatorParts[0]),
                int.Parse(differentiatorParts[1]),
                int.Parse(differentiatorParts[2])
            ),
            uint.Parse(hashOffsetParts[0]),
            int.Parse(hashOffsetParts[1]),
            key
        );
    }
}
