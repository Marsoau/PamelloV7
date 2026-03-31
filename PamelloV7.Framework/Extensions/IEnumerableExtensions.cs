namespace PamelloV7.Framework.Extensions;

public static class IEnumerableExtensions
{
    public static TType? ElementAtValueOrDefault<TType>(this IEnumerable<TType> enumerable, string value, bool includeLastEmpty = false, Func<IEnumerable<TType>, int?>? getCurrent = null) {
        var index = enumerable.TranslateValueIndex(value, includeLastEmpty, getCurrent);
        return index >= 0 ? enumerable.ElementAt(index) : default;
    }

    public static int TranslateValueIndex<TType>(this IEnumerable<TType> enumerable, string value, bool includeLastEmpty = false, Func<IEnumerable<TType>, int?>? getCurrentExternal = null, Func<string, int?>? getOther = null) {
        var items = enumerable.ToList();
        if (items.Count == 0) return includeLastEmpty ? 0 : -1;
        
        if (int.TryParse(value, out var result)) {
            if (result == 0) return 0;
            
            if (result > 0) result -= 1;
            
            var originallyZero = result == 0;
            
            result %= items.Count;
            
            if (result < 0) result += items.Count;
            else if (includeLastEmpty && result == 0 && !originallyZero) result = items.Count;
            
            return result;
        }

        return value switch {
            "first" or "start" => 0,
            "last" or "end" => includeLastEmpty ? items.Count : items.Count - 1,
            "random" => Random.Shared.Next(items.Count),
            "current" => GetCurrent(),
            "next" => GetNext(),
            "prev" or "previous" => GetPrev(),
            _ => getOther?.Invoke(value) ?? -2
        };

        int GetCurrent() {
            return getCurrentExternal?.Invoke(items) ?? -1;
        }
        int GetNext() {
            var current = GetCurrent();
            if (current < 0) return -1;
            
            return current + 1 % items.Count;
        }
        int GetPrev() {
            var current = GetCurrent();
            if (current < 0) return -1;
            var prev = current - 1;
            
            return prev < 0 ? items.Count - 1 : prev;
        }
    }
}
