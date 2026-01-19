namespace PamelloV7.Core.Extensions;

public static class IEnumerableExtensions
{
    public static TType? ElementAtValueOrDefault<TType>(this IEnumerable<TType> enumerable, string value, Func<IEnumerable<TType>, int>? getCurrent = null) {
        var index = enumerable.TranslateValueIndex(value, getCurrent);
        return index >= 0 ? enumerable.ElementAt(index) : default;
    }

    public static int TranslateValueIndex<TType>(this IEnumerable<TType> enumerable, string value, Func<IEnumerable<TType>, int>? getCurrent = null) {
        var count = enumerable.Count();
        if (count == 0) return -1;
        
        if (int.TryParse(value, out var result)) {
            if (result > 0) result -= 1;
            result %= count;
            
            if (result < 0) result += count;
            
            return result;
        }

        return value switch {
            "first" => 0,
            "last" => count - 1,
            "random" => Random.Shared.Next(count),
            "current" => getCurrent is not null ? getCurrent(enumerable) : -1,
            _ => -2
        };
    }
}
