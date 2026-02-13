using System.Text;
using Discord;
using Discord.WebSocket;
using PamelloV7.Core.Entities;

namespace PamelloV7.Module.Marsoau.Discord.Strings
{
    public static class DiscordString
    {
        public static string User(IPamelloUser? user) {
            var discordAuthorization = user?.SelectedAuthorization;
            if (discordAuthorization is not null && discordAuthorization.PK.Platform != "discord") discordAuthorization = null;
            
            discordAuthorization = user?.Authorizations.FirstOrDefault(auth => auth.PK.Platform == "discord");
            if (discordAuthorization is null) return "<@0>";
            
            return $"<@{discordAuthorization.PK.Key}>";
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
        public static string Spoiler(object? obj) {
            return obj is not null ? $"||{obj}||" : "";
        }
        
        public static string Progress(double progress, int length, bool percent = false) {
            var done = (int)(progress * length);
            
            var sb = new StringBuilder();
            
            for (var i = 0; i < done; i++) sb.Append('▬');
            for (var i = done; i < length; i++) sb.Append(' ');
            
            if (!percent) return Code($"[{sb}]");
            return Code($"[{sb}] {progress * 100:0.00}%");
        }
        
        public static string Emote(Emote? emote) {
            return emote is not null ? $"<:{emote.Name}:{emote.Id}>" : "";
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
