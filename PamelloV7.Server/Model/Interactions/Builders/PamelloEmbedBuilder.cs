using Discord;

namespace PamelloV7.Server.Model.Interactions.Builders
{
    public static class PamelloEmbedBuilder
    {
        public static Embed BuildInfo(string header, string message = "") {
            return Info(header, message).Build();
        }
        public static Embed BuildPage(string header, string message = "", int? page = null, int? totalPages = null) {
            return Page(header, message, page, totalPages).Build();
        }
        public static Embed BuildError(string message) {
            return Error(message).Build();
        }
        public static Embed BuildException(string message) {
            return Exception(message).Build();
        }

        public static EmbedBuilder Info(string header, string message) {
            return new EmbedBuilder() {
                Title = header,
                Description = message,
            }
            .WithColor(0x00A795AC);
        }
        public static EmbedBuilder Page(string header, string content, int? page = null, int? totalPages = null) {
            return Info(header, content)
                .WithFooter($"{
                    (page is not null ? $"page {page}" : "")
                }{
                    (totalPages is not null ? $" / {totalPages}" : "")
                }");
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
