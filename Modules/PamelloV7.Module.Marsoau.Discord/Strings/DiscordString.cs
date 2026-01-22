using Discord;
using Discord.WebSocket;
using PamelloV7.Core.Entities;

namespace PamelloV7.Module.Marsoau.Discord.Strings
{
    public class DiscordString
    {
        private string _str;

        public DiscordString() : this("") { }

        public DiscordString(object? obj, bool skipEcranation = true) {
            if (obj is DiscordString discordStr) {
                _str = discordStr._str;
            }
            else if (skipEcranation) {
                _str = obj?.ToString() ?? "";
            }
            else _str = Ecranate(obj);
        }
        public DiscordString(IPamelloUser user) {
            var discordAdderId = user.Authorizations.FirstOrDefault(auth => auth.PK.Platform == "discord")?.PK.Key;
            _str = $"<@{discordAdderId ?? "0"}>";
        }
        public DiscordString(DateTime date) {
            _str = $"<t:{((DateTimeOffset)date).ToUnixTimeSeconds()}:f>";
        }

        public DiscordString Bold() {
            _str = $"**{_str}**";
            return this;
        }
        public DiscordString Italic() {
            _str = $"*{_str}*";
            return this;
        }
        public DiscordString Crossed() {
            _str = $"~~{_str}~~";
            return this;
        }
        public DiscordString Quote() {
            _str = $">{_str}";
            return this;
        }
        public DiscordString Code() {
            _str = $"`{_str}`";
            return this;
        }
        public DiscordString Spoiler() {
            _str = $"||{_str}||";
            return this;
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

        public static DiscordString Bold(object obj) {
            return new DiscordString(obj).Bold();
        }
        public static DiscordString Italic(object obj) {
            return new DiscordString(obj).Italic();
        }
        public static DiscordString Crossed(object obj) {
            return new DiscordString(obj).Crossed();
        }
        public static DiscordString Quote(object obj) {
            return new DiscordString(obj).Quote();
        }
        public static DiscordString Code(object obj) {
            return new DiscordString(obj).Code();
        }
        public static DiscordString Spoiler(object obj) {
            return new DiscordString(obj).Spoiler();
        }
        public static DiscordString Emote(Emote emote) {
            return new DiscordString($"<:{emote.Name}:{emote.Id}>");
        }

        public static string Url(object obj, string url) {
            if (url?.Length > 0) return $"[{obj}]({url})";
            return obj.ToString() ?? "";
        }

        public override string ToString() {
            return _str;
        }

        public static DiscordString operator +(DiscordString discordStr, string? str) {
            return new DiscordString(discordStr._str + str, true);
        }
        public static DiscordString operator +(string? str, DiscordString discordStr) {
            return new DiscordString(str + discordStr._str, true);
        }
        public static DiscordString operator +(DiscordString strA, DiscordString strB) {
            return new DiscordString(strA._str + strB._str, true);
        }
    }
}
