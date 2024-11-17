using PamelloV7.Server.Enumerators;
using PamelloV7.Server.Model;
using System.Diagnostics;
using System.Text;

namespace PamelloV7.Server.Services
{
    public class YoutubeDownloadService
    {
		private readonly PamelloEventsService _events;

		private readonly Dictionary<int, Task<EDownloadResult>> _downloadingSongs;

		public YoutubeDownloadService(
			PamelloEventsService events
		) {
			_events = events;

			_downloadingSongs = new Dictionary<int, Task<EDownloadResult>>();
		}

		public bool IsDownloading(PamelloSong song) {
			return _downloadingSongs.ContainsKey(song.Id);
		}

		public async Task<EDownloadResult> DownloadFromYoutubeAsync(PamelloSong song, bool forceDownload = false) {
			if (IsDownloading(song)) {
				return await _downloadingSongs[song.Id];
			}
			if (song.IsDownloaded) {
				if (!forceDownload) return EDownloadResult.Success;

				File.Delete($@"{AppContext.BaseDirectory}Data\Music\{song.Id}.opus");
			}

			var downloadTask = new TaskCompletionSource<EDownloadResult>();

			_downloadingSongs.Add(song.Id, downloadTask.Task);
			_events.DownloadStart(song);

			using var process = new Process();
			process.StartInfo = new ProcessStartInfo() {
				FileName = $@"{AppContext.BaseDirectory}yt-dlp.exe",
				Arguments = $@"--quiet --newline --progress --no-wait-for-video --no-keep-video --no-audio-multistreams --extract-audio --output ""{AppContext.BaseDirectory}Data\Music\{song.Id}"" --progress-template ""download:%(progress.downloaded_bytes)s/%(progress.total_bytes)s"" https://www.youtube.com/watch?v={song.YoutubeId}",
				StandardOutputEncoding = Encoding.Unicode,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};

            if (!process.Start()) {
				return EDownloadResult.CantStart;
			}

			var sr = new StreamReader(process.StandardOutput.BaseStream);

			string[] progress;
			int bytesDownloaded = 0;
			int bytesTotal = 0;

			while (!sr.EndOfStream) {
				progress = (await sr.ReadLineAsync())?.Split('/') ?? ["0", "0"];
				if (!int.TryParse(progress[0], out bytesDownloaded)) bytesDownloaded = 0;
				if (!int.TryParse(progress[1], out bytesTotal)) bytesTotal = 0;

				_events.DownloadProggress(song, (double)bytesDownloaded / bytesTotal);
			}

			await process.WaitForExitAsync();

			var finalResult = process.ExitCode == 0 ? EDownloadResult.Success : EDownloadResult.UnknownError;

			downloadTask.SetResult(finalResult);

			_downloadingSongs.Remove(song.Id);
			_events.DownloadEnd(song, finalResult);

			return finalResult;
		}
    }
}
