using Discord;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Model.Discord;
using System.Text;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Model.Audio;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Server.Config;
using PamelloV7.Server.Extensions;
using PamelloV7.Server.Model.Audio.Modules.Pamello;
using PamelloV7.Server.Model.Audio.Speakers;

namespace PamelloV7.Server.Model.Interactions.Builders
{
    public static class PamelloEmbedBuilder
    {
        public static Embed BuildInfo(string header, string message = "", string description = "") {
            return Info(header, message, description).Build();
        }
        public static Embed BuildQueuePage(IPamelloQueue queue, int page, int elementCount) {
            return QueuePage(queue, page, elementCount).Build();;
        }
        public static Embed BuildPage<T>(string header, IEnumerable<T> content, Action<StringBuilder, int, T> writeElement, int count = 20, int page = 0) {
            return Page(header, content, writeElement, page, count).Build();
        }
        public static Embed BuildError(string message) {
            return Error(message).Build();
        }
        public static Embed BuildException(string message) {
            return Exception(message).Build();
        }
        public static Embed BuildSongInfo(IPamelloSong song) {
            return SongInfo(song).Build();
        }
        public static Embed BuildPlayerInfo(IPamelloPlayer player) {
            return PlayerInfo(player).Build();
        }
        public static Embed BuildPlaylistInfo(IPamelloPlaylist playlist) {
            return PlaylistInfo(playlist).Build();
        }
        public static Embed BuildSpeakerInfo(IPamelloSpeaker speaker) {
            return SpeakerInfo(speaker).Build();
        }

        public static EmbedBuilder Info(string header, string message, string description = "") {
            return new EmbedBuilder() {
                Title = header,
                Description = message,
                Footer = new EmbedFooterBuilder() {
                    Text = description,
                }
            }
            .WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Info.Color));
        }
        
        public static EmbedBuilder QueuePage(IPamelloQueue queue, int page, int elementCount) {
            var embedBuilder = PamelloEmbedBuilder.Page(
                "Queue",
                queue.Entries,
                (sb, pos, entry) => {
                    sb.Append(DiscordString.Code(pos));
                    if (queue.Position == pos) {
                        sb.Append(
                            (" > " + entry.Song.ToDiscordString()).Bold()
                        );
                        sb.AppendLine();
                        sb.Append("Added by " + "none");
                    }
                    else {
                        sb.Append(" - ");
                        sb.Append(entry.Song.ToDiscordString());
                    }

                    sb.AppendLine();
                }
            );
            embedBuilder.Footer.Text += $" | {queue.Player.ToDiscordFooterString()}";

            return embedBuilder;
        }
        public static EmbedBuilder Page<T>(string header, IEnumerable<T> content, Action<StringBuilder, int, T> writeElement, int page = 0, int count = 20) {
            var sb = new StringBuilder();

            if (page < 0) page = 0;

            var totalPages = content.Count() / count + (content.Count() % count != 0 ? 1 : 0);
            if (totalPages == 0) totalPages = 1;

            if (page >= totalPages) page = totalPages - 1;

            var start = page * count;
            var end = (page + 1) * count;

            if (end > content.Count()) {
                end = content.Count();
            }

            for (int pos = start; pos < end; pos++) {
                writeElement(sb, pos, content.ElementAt(pos));
            }

            return new EmbedBuilder() {
                Title = header,
                Description = sb.Length != 0 ? sb.ToString() : DiscordString.Italic("Empty").ToString(),
                Footer = new EmbedFooterBuilder() {
                    Text = $"Page {page + 1} / {totalPages} ({content.Count()})"
                }
            }
            .WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Info.Color));
        }

        public static EmbedBuilder SongInfo(IPamelloSong song) {
            return new EmbedBuilder() {
                Title = song.Name,
                Url = $"https://www.youtube.com/watch?v=song.YoutubeId",
                ThumbnailUrl = song.CoverUrl,
                Footer = new EmbedFooterBuilder() {
                    Text = $"Id: {song.Id} | Added: {song.AddedAt.ToLocalTime()}"
                },
                Fields = [
                    new EmbedFieldBuilder() {
                        Name = "Played",
                        Value = 0,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder() {
                        Name = "Favorite By",
                        Value = song.FavoriteBy.Count,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder() {
                        Name = "Added By",
                        Value = new DiscordString(song.AddedBy),
                        IsInline = true,
                    }
                ]
            }
            .WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Info.Color));
        }

        public static EmbedBuilder PlayerInfo(IPamelloPlayer player) {
            var fields = new List<EmbedFieldBuilder>();

            var currentAudio = player.Queue.Audio;

            if (currentAudio is not null) {
                var positionSb = new StringBuilder();

                var length = 20;
                var lineLength = currentAudio.Position.TotalSeconds / (currentAudio.Duration.TotalSeconds / length);

                positionSb.Append($"`{currentAudio.Position.ToShortString()}`");

                positionSb.Append(" `[");
                for (int i = 0; i < lineLength; i++) {
                    positionSb.Append('-');
                }
                positionSb.Append('|');
                for (int i = lineLength; i < length - 1; i++) {
                    positionSb.Append(' ');
                }
                positionSb.Append("]` ");

                positionSb.Append($"`{currentAudio.Duration.ToShortString()}`");

                fields.Add(new EmbedFieldBuilder() {
                    Name = "Position",
                    Value = positionSb
                });
                fields.Add(new EmbedFieldBuilder() {
                    Name = "Song",
                    Value = currentAudio.Song.ToDiscordString(),
                });
                var episode = currentAudio.GetCurrentEpisode();
                if (episode is not null) {
                    fields.Add(new EmbedFieldBuilder() {
                        Name = "Episode",
                        Value = DiscordString.Code(currentAudio.GetCurrentEpisodePosition() ?? -1) + " " + episode.ToDiscordString()
                    });
                }
            }
            fields.Add(new EmbedFieldBuilder() {
                Name = "Queue Modes",
                Value = $"""
                         Random: {DiscordString.Code(player.Queue.IsRandom ? "Enabled" : "Disabled")}
                         Reversed: {DiscordString.Code(player.Queue.IsReversed ? "Enabled" : "Disabled")}
                         No Leftovers: {DiscordString.Code(player.Queue.IsNoLeftovers ? "Enabled" : "Disabled")}
                         Feed Random: {DiscordString.Code(player.Queue.IsFeedRandom ? "Enabled" : "Disabled")}
                         """
            });
            fields.Add(new EmbedFieldBuilder()
            {
                Name = "Speakers",
                Value = $"""
                         Discord: {DiscordString.Code(0)} Speakers
                         Internet: {DiscordString.Code(0)} Speakers ({DiscordString.Code(0)} Listeners)
                         """
            });
            fields.Add(new EmbedFieldBuilder() {
                Name = "Status",
                Value = player.State.ToString(),
                IsInline = true,
            });
            fields.Add(new EmbedFieldBuilder() {
                Name = "Paused",
                Value = player.IsPaused ? "Yes" : "No",
                IsInline = true,
            });
            fields.Add(new EmbedFieldBuilder() {
                Name = "Private",
                Value = player.IsProtected ? "Yes" : "No",
                IsInline = true,
            });
            return new EmbedBuilder() {
                Title = player.Name,
                Fields = fields
            }
            .WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Info.Color));
        }
        public static EmbedBuilder PlaylistInfo(IPamelloPlaylist playlist) {
            return new EmbedBuilder() {
                Title = playlist.Name,
                Footer = new EmbedFooterBuilder() {
                    Text = $"Id: {playlist.Id}"
                },
                Fields = [
                    new EmbedFieldBuilder {
                        Name = "Songs Count",
                        Value = playlist.Songs.Count,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder() {
                        Name = "Protected",
                        Value = playlist.IsProtected,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder() {
                        Name = "Added By",
                        Value = new DiscordString(playlist.Owner),
                        IsInline = true,
                    }
                ]
            }
            .WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Info.Color));
        }

        public static EmbedBuilder SpeakerInfo(IPamelloSpeaker speaker)
        {
            EmbedBuilder? embedBuilder = null;
            /*
            if (speaker is PamelloDiscordSpeaker discordSpeaker)
            {
                embedBuilder = new EmbedBuilder()
                {
                    Title = "Discord speaker",
                    Fields = [
                        new EmbedFieldBuilder() {
                            Name = "Player",
                            Value = speaker.Player
                        }
                    ]
                };
            }
            */
            if (speaker is IPamelloInternetSpeaker internetSpeaker)
            {
                embedBuilder = new EmbedBuilder()
                {
                    Title = "Internet speaker",
                    Fields = [
                        new EmbedFieldBuilder() {
                            Name = "Player",
                            Value = speaker.Player
                        },
                        new EmbedFieldBuilder() {
                            Name = "Listeners",
                            Value = internetSpeaker.ListenersCount
                        }
                    ],
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = $"Id: {speaker.Id}"
                    }
                };
            }
            else throw new PamelloException("Unknown speaker type, cant build embed");
            
            return embedBuilder.WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Info.Color));
        }

        public static EmbedBuilder Error(string message) {
            return new EmbedBuilder() {
                Title = "Error",
                Description = message,
            }
            .WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Error.Color));
        }
        public static EmbedBuilder Exception(string message) {
            return new EmbedBuilder() {
                Title = "Exception",
                Description = message,
            }
            .WithColor(Color.Parse(PamelloServerConfig.Root.Discord.MessageStyles.Exception.Color));
        }
    }
}
