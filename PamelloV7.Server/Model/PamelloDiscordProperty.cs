namespace PamelloV7.Server.Model
{
    public class PamelloDiscordProperty
    {
        public string Name { get; }
        public string Description { get; }

        private PamelloDiscordProperty(string name, string description) {
            Name = name;
            Description = description;
        }

        public static PamelloDiscordProperty PlayerValue => new PamelloDiscordProperty(
            "player",
            "Player id or name"
        );
        public static PamelloDiscordProperty SongValue => new PamelloDiscordProperty(
            "song",
            "Song id/youtube-url/associacion/name"
        );
        public static PamelloDiscordProperty SongEpisodeValue => new PamelloDiscordProperty(
            "song",
            "Episode position"
        );
    }
}
