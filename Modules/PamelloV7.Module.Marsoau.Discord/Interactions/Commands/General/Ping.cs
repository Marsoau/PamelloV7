using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.General;

[Map]
public class Ping : DiscordCommand
{
    [SlashCommand("ping", "Ping the bot", runMode: RunMode.Async)]
    public async Task ExecuteAsync() {
        var vc = Context.Guild.GetUser(Context.DiscordUser.Id).VoiceChannel;
        if (vc is null) {
            await RespondAsync("Not in a voice channel");
            return;
        }

        var ac = await vc.ConnectAsync();
        if (ac is null) {
            await RespondAsync("Couldnt connect to voice channel");
            return;
        }
        
        var stream = ac.CreatePCMStream(AudioApplication.Music);
        if (stream is null) {
            await RespondAsync("Couldnt create audio stream");
            return;
        }
        
        var audio = Services.GetRequiredService<IPamelloAudioSystem>();
        var song = await GetSingleRequiredAsync<IPamelloSong>("18");

        var songAudio = audio.RegisterModule(new SongAudio(song, Services));
        var pump = audio.RegisterModule(new AudioPump(4096));

        pump.Input.ConnectedPoint = songAudio.Output;
        pump.Output.ProcessAudio = (audio, wait, cts) => {
            stream.WriteAsync(audio, cts).AsTask().GetAwaiter().GetResult();
            return true;
        };
        
        await RespondAsync("definitely playing");
        
        await songAudio.TryInitialize();

        Console.WriteLine("playing");
        await Task.Run(() => {
            while (!songAudio.IsEnded) {
                pump.Pump();
            }
        });
        Console.WriteLine("end");
        
        await FollowupAsync("end");
        
        return;
        await RespondUpdatableAsync(message => {
            message.Components = PamelloComponentBuilders.Info("Pong!", $"Hi {Context.User.ToDiscordString()}!").Build();
        }, Context.User);
    }
}
