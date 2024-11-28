namespace PamelloV7.Server.Model.Discord
{
    public class DiscordString
    {
        private string _str;

        public DiscordString(object obj, bool skipEcranation = false) {
            if (obj is DiscordString discordStr) {
                _str = discordStr._str;
            }
            else if (skipEcranation) {
                _str = obj.ToString() ?? "";
            }
            else _str = Ecranate(obj);
        }
        public DiscordString(PamelloUser user) {
            _str = $"<@{user.DiscordUser.Id}>";
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

        public static string Ecranate(object obj) {
            return obj?.ToString()
                ?.Replace("_", @"\_")
                ?.Replace("`", @"\`")
                ?.Replace("~", @"\~")
                ?.Replace("*", @"\*")
                ?.Replace("|", @"\|")
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

        public static string Url(object obj, string url) {
            return $"[{obj}]({url})";
        }

        public override string ToString() {
            return _str;
        }

        public static DiscordString operator +(DiscordString discordStr, string str) {
            return new DiscordString(discordStr._str + str, true);
        }
        public static DiscordString operator +(string str, DiscordString discordStr) {
            return new DiscordString(str + discordStr._str, true);
        }
        public static DiscordString operator +(DiscordString strA, DiscordString strB) {
            return new DiscordString(strA._str + strB._str, true);
        }
    }
}
