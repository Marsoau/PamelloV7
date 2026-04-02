using System.Text;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Module.Marsoau.NetCord.Strings
{
    public static class DiscordString
    {
        public static string User(IPamelloUser? user) {
            var discordAuthorization = user?.SelectedAuthorization;
            if (discordAuthorization is not null && discordAuthorization.PK.Platform != "discord") discordAuthorization = null;
            
            discordAuthorization = user?.Authorizations.FirstOrDefault(auth => auth.PK.Platform == "discord");
            if (discordAuthorization is null) return "<@0>";
            
            return User(ulong.Parse(discordAuthorization.PK.Key));
        }
        public static string User(ulong userId) {
            return $"<@{userId}>";
        }
        public static string Time(DateTime date) {
            return $"<t:{((DateTimeOffset)date).ToUnixTimeSeconds()}:f>";
        }

        public static string None(object? obj) {
            return obj?.ToString() ?? "";
        }
        public static string Bold(object? obj) {
            return obj is not null ? $"**{obj}**" : "";
        }
        public static string Italic(object? obj) {
            return obj is not null ? $"*{obj}*" : "";
        }
        public static string Crossed(object? obj) {
            return obj is not null ? $"~~{obj}~~" : "";
        }
        public static string Quote(object? obj) {
            return obj is not null ? $">{obj}" : "";
        }
        public static string Code(object? obj) {
            return obj is not null ? $"`{obj}`" : "";
        }
        public static string CodeBlock(object? obj, string language = "plaintext") {
            return obj is not null ? $"```{language}\n{obj}\n```" : "";
        }
        public static string Spoiler(object? obj) {
            return obj is not null ? $"||{obj}||" : "";
        }

        private static readonly char NoChar = '\u2800';
        private static readonly char[] ProgressChars = [
            '\u2840', '\u2844', '\u2846', '\u2847', '\u28C7', '\u28E7', '\u28F7', '\u28FF'
        ];
        
        public static string Progress(double progress, int length, bool percent = false) {
            var sb = new StringBuilder();
            
            var size = length * ProgressChars.Length;
            var filled = (int)(size * progress);
            
            for (var i = 0; i < length; i++) {
                if (i < filled / ProgressChars.Length) {
                    sb.Append(ProgressChars.Last());
                    continue;
                }
                if (i == filled / ProgressChars.Length && i != 0) {
                    sb.Append(ProgressChars[filled % ProgressChars.Length]);
                    continue;
                }
                
                sb.Append(NoChar);
            }
            
            if (!percent) return Code($"[{sb}]");
            return Code($"[{sb}] {progress * 100:0.00}%");
        }
        
        public static string Ecranate(object? obj) {
            return obj?.ToString()?
                .Replace("_", @"\_")
                .Replace("`", @"\`")
                .Replace("~~", @"\~\~")
                .Replace("*", @"\*")
                .Replace("||", @"\|\|")
                ?? "";
        }

        public static string Url(object obj, string url) {
            if (url?.Length > 0) return $"[{obj}]({url})";
            return obj.ToString() ?? "";
        }
    }
}
