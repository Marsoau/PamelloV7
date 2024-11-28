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
        public static Embed BuildPage<T>(string header, IReadOnlyList<T> content, Action<StringBuilder, int, T> writeElement, int count = 20, int page = 0) {
            return Page(header, content, writeElement, page, count).Build();
        }
        public static Embed BuildError(string message) {
            return Error(message).Build();
        }
        public static Embed BuildException(string message) {
            return Exception(message).Build();
        }
        public static Embed BuildSongInfo(PamelloSong song) {
            return SongInfo(song).Build();
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
        public static EmbedBuilder Page<T>(string header, IReadOnlyList<T> content, Action<StringBuilder, int, T> writeElement, int page = 0, int count = 20) {
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

            for (int pos = start; pos < end; pos++) {
                writeElement(sb, pos, content[pos]);
            }

            return new EmbedBuilder() {
                Title = header,
                Description = sb.Length != 0 ? sb.ToString() : DiscordString.Italic("Empty").ToString(),
                Footer = new EmbedFooterBuilder() {
                    Text = $"Page {page + 1} / {totalPages} ({content.Count})"
                }
            }
            .WithColor(0x00A795AC);
        }

        public static EmbedBuilder SongInfo(PamelloSong song) {
            return new EmbedBuilder() {
                Title = song.Name,
                Url = $"https://www.youtube.com/watch?v={song.YoutubeId}",
                ThumbnailUrl = song.CoverUrl,
                Footer = new EmbedFooterBuilder() {
                    Text = $"Id: {song.Id} | Added: {song.AddedAt.ToLocalTime()}"
                },
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = "Played",
                        Value = song.PlayCount,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder() {
                        Name = "Favorite By",
                        Value = song.FavoritedBy.Count,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder() {
                        Name = "Added By",
                        Value = new DiscordString(song.AddedBy),
                        IsInline = true,
                    }
                }
            }
            .WithColor(0x00A795AC);
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
