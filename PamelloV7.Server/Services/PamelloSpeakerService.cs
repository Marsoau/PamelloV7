using Discord.WebSocket;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Repositories;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Repositories.Dynamic;

namespace PamelloV7.Server.Services.Deleted
{
    public class PamelloSpeakerService
    {
        private readonly IServiceProvider _services;

        private readonly DiscordClientService _discordClients;

        private readonly PamelloPlayerRepository _players;

        private readonly List<PamelloSpeaker> _speakers;

        public PamelloSpeakerService(IServiceProvider services) {
            _services = services;

            _discordClients = services.GetRequiredService<DiscordClientService>();

            _players = services.GetRequiredService<PamelloPlayerRepository>();

            _speakers = new List<PamelloSpeaker>();
        }

        private void Speaker_Terminated(PamelloSpeaker speaker) {
            _speakers.Remove(speaker);
        }

        public async Task DisconnectDiscord(PamelloPlayer player, ulong vcId) {
            foreach (var speaker in _speakers) {
                if (speaker is not PamelloDiscordSpeaker discordSpeaker) continue;

                if (discordSpeaker.Player == player && discordSpeaker.Voice.Id == vcId) {
                    await speaker.DisposeAsync();
                    return;
                }
            }
        }

        public bool DoesPlayerHasSpeakers(PamelloPlayer player) {
            foreach (var speaker in _speakers) {
                if (speaker.Player == player && speaker.IsActive) {
                    return true;
                }
            }

            return false;
        }

        public PamelloInternetSpeaker? GetInternetSpeaker(string channel) {
            foreach (var speaker in _speakers) {
                if (speaker is not PamelloInternetSpeaker internetSpeaker) continue;
                
                if (internetSpeaker.Channel == channel) return internetSpeaker;
            }

            return null;
        }
    }
}
