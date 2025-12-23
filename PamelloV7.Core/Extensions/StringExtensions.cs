namespace PamelloV7.Core.Extensions;

public static class StringExtensions
{
    public static string[] SplitArgs(this string str, char separator) {
        var argsList = new List<string>();
        var lastSeparatorIndex = str.Length;
        
        var quotesDepth = 0;

        for (var i = str.Length - 1; i >= 0; i--) {
            switch (str[i]) {
                case ')': quotesDepth++; continue;
                case '(': quotesDepth--; continue;
                case not ',': continue;
            }
            
            if (quotesDepth > 0) continue;
            
            argsList.Add(str[(i + 1)..lastSeparatorIndex]);
            lastSeparatorIndex = i;
        }
        if (lastSeparatorIndex < str.Length) argsList.Add(str[..lastSeparatorIndex]);
        if (argsList.Count == 0) return [str];
        
        argsList.Reverse();
        return argsList.ToArray();
    }

    public static (string, string) SplitInTwo(this string str, char separator) {
        var quotesDepth = 0;

        for (var i = str.Length - 1; i >= 0; i--) {
            switch (str[i]) {
                case ')': quotesDepth++; continue;
                case '(': quotesDepth--; continue;
                case not ',': continue;
            }
            
            if (quotesDepth > 0) continue;
            
            return (
                str[..i],
                str[(i + 1)..]
            );
        }

        return (str, "");
    }
}
