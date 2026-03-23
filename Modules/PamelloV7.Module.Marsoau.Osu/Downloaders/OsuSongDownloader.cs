using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.Osu.Services;

namespace PamelloV7.Module.Marsoau.Osu.Downloaders;

[SongDownloader("osu")]
public class OsuSongDownloader : SongDownloader
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OsuSongDownloader(IServiceProvider services, SongSource source) : base(services, source) {
        _httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    }

    protected override async Task<EDownloadResult> InternalDownloadAsync(FileInfo file) {
        if (!int.TryParse(Source.PK.Key, out var id)) throw new Exception($"Invalid osu beatmap set id, cannot parse {Source.PK.Key} to int");
        var downloadUrl = $"https://catboy.best/d/{id}";
        var response = await _httpClientFactory.CreateClient().GetAsync(downloadUrl);
        
        using var oszStream = new MemoryStream();
        using var downloadStream = await response.Content.ReadAsStreamAsync();
        
        var total = response.Content.Headers.ContentLength ?? 0;
        
        var buffer = new byte[total / 20];
        
        int bytesRead;
        var totalBytesRead = 0;
        while ((totalBytesRead += bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) < total || bytesRead > 0) {
            await oszStream.WriteAsync(buffer, 0, bytesRead);
            Progress = (double)totalBytesRead / total;
        }
        
        oszStream.Position = 0;

        using var zipArchive = new ZipArchive(oszStream, ZipArchiveMode.Read);
        
        Output.Write($"zip: {zipArchive.Entries.Count};");
        
        var audioEntry = zipArchive.Entries.FirstOrDefault(entry => entry.FullName == "audio.mp3")
            ?? zipArchive.Entries.FirstOrDefault(entry => entry.FullName.EndsWith(".mp3"));
        if (audioEntry is null) throw new Exception("Cant find audio file in the zip archive");
        
        audioEntry.ExtractToFile(file.FullName, true);
        
        return EDownloadResult.Success;
    }
}
