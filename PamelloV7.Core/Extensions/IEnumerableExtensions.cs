namespace PamelloV7.Core.Extensions;

public static class IEnumerableExtensions
{
    public static TType? ElementAtValueOrDefault<TType>(this IEnumerable<TType> enumerable, string value, bool includeLastEmpty = false, Func<IEnumerable<TType>, int?>? getCurrent = null) {
        var index = enumerable.TranslateValueIndex(value, includeLastEmpty, getCurrent);
        return index >= 0 ? enumerable.ElementAt(index) : default;
    }

    public static int TranslateValueIndex<TType>(this IEnumerable<TType> enumerable, string value, bool includeLastEmpty = false, Func<IEnumerable<TType>, int?>? getCurrent = null, Func<string, int?>? getOther = null) {
        var count = enumerable.Count();
        if (count == 0) return includeLastEmpty ? 0 : -1;
        
        if (int.TryParse(value, out var result)) {
            if (result == 0) return 0;
            
            if (result > 0) result -= 1;
            
            var originallyZero = result == 0;
            
            result %= count;
            
            if (result < 0) result += count;
            else if (includeLastEmpty && result == 0 && !originallyZero) result = count;
            
            return result;
        }

        return value switch {
            "first" or "start" => 0,
            "last" or "end" => includeLastEmpty ? count : count - 1,
            "random" => Random.Shared.Next(count),
            "current" => getCurrent?.Invoke(enumerable) ?? -1,
            "next" => getCurrent is null ? -1 : getCurrent(enumerable).Value + 1,
            "prev" or "previous" => getCurrent is null ? -1 : getCurrent(enumerable).Value - 1,
            _ => getOther?.Invoke(value) ?? -2
        };
    }
}
