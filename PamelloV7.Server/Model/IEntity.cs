using PamelloV7.Server.Model.Discord;

namespace PamelloV7.Server.Model
{
    public interface IEntity
    {
        public int Id { get; }
        public string Name { get; }

        public DiscordString ToDiscordString();

        public object? DTO { get; }
    }
}
