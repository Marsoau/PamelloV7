using Discord;
using PamelloV7.Server.Model.Discord;
using System.Text;

namespace PamelloV7.Server.Model.Interactions.Builders
{
    public static class PamelloEmbedBuilder
    {
        public static Embed BuildInfo(string header, string message = "", string description = "") {
            return Info(header, message, description).Build();
        }
        public static Embed BuildPage<T>(string header, List<T> content, Action<StringBuilder, int, T> tostr, int count = 20, int page = 0) {
            return Page(header, content, tostr, count, page).Build();
        }
        public static Embed BuildError(string message) {
            return Error(message).Build();
        }
        public static Embed BuildException(string message) {
            return Exception(message).Build();
        }

        public static EmbedBuilder Info(string header, string message, string description = "") {
            return new EmbedBuilder() {
                Title = header,
                Description = message,
                Footer = new EmbedFooterBuilder() {
                    Text = description,
                }
            }
            .WithColor(0x00A795AC);
        }
        public static EmbedBuilder Page<T>(string header, List<T> content, Action<StringBuilder, int, T> tostr, int count = 20, int page = 0) {
            var sb = new StringBuilder();

            if (page < 0) page = 0;

            var totalPages = content.Count / count + (content.Count % count != 0 ? 1 : 0);
            if (totalPages == 0) totalPages = 1;

            if (page >= totalPages) page = totalPages - 1;

            var start = page * count;
            var end = (page + 1) * count;

            if (end > content.Count) {
                end = content.Count;
            }

            var pageSongs = content.Slice(start, end - start);
            var pos = page * count;

            foreach (var element in content) {
                tostr.Invoke(sb, pos, element);
                pos++;
            }

            return new EmbedBuilder() {
                Title = header,
                Description = sb.Length != 0 ? sb.ToString() : DiscordString.Italic("Empty").ToString(),
                Footer = new EmbedFooterBuilder() {
                    Text = $"Page {page + 1} / {totalPages}"
                }
            };
        }
        public static EmbedBuilder Error(string message) {
            return new EmbedBuilder() {
                Title = "Error",
                Description = message,
            }
            .WithColor(0x00484848);
        }
        public static EmbedBuilder Exception(string message) {
            return new EmbedBuilder() {
                Title = "Exception",
                Description = message,
            }
            .WithColor(0x00FF3030);
        }
    }
}
