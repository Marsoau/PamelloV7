using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Server.Model;
using System.Diagnostics;
using System.Text;

namespace PamelloV7.Server.Services
{
	public record YoutubeDownloadItem
	{
		public required PamelloSong Song;
		public required Task<EDownloadResult> Task;
		public double Progress;
	}

    public class YoutubeDownloadService
    {
		private readonly PamelloEventsService _events;

		private readonly List<YoutubeDownloadItem> _downloads;

		public YoutubeDownloadService(
			PamelloEventsService events
		) {
			_events = events;

			_downloads = new List<YoutubeDownloadItem>();
		}

		private YoutubeDownloadItem? GetDownload(PamelloSong song) {
			foreach (var download in _downloads) {
                if (download.Song == song) {
					return download;
                }
            }
			return null;
		}

		public bool IsDownloading(PamelloSong song) {
			return GetDownload(song) is not null;
		}
		public double GetProgress(PamelloSong song) {
			var download = GetDownload(song);
			return download?.Progress ?? 0;
		}

		public async Task<EDownloadResult> DownloadFromYoutubeAsync(PamelloSong song, bool forceDownload = false) {
			var download = GetDownload(song);
			if (download is not null) {
				return await download.Task;
			}
			if (song.IsDownloaded) {
				if (!forceDownload) return EDownloadResult.Success;

				File.Delete($@"{AppContext.BaseDirectory}Data/Music/{song.Id}.opus");
			}

			var downloadTask = new TaskCompletionSource<EDownloadResult>();

			download = new YoutubeDownloadItem() {
				Song = song,
				Task = downloadTask.Task,
				Progress = 0
			};

			_downloads.Add(download);
			_events.Broadcast(new SongDownloadStarted() {
				SongId = song.Id,
			});

			if (!Directory.Exists($"{AppContext.BaseDirectory}Data/Music")) {
				Directory.CreateDirectory($"{AppContext.BaseDirectory}Data/Music");
			}

			using var process = new Process();
			process.StartInfo = new ProcessStartInfo() {
				FileName = $@"yt-dlp",
				Arguments = $@"--quiet --newline --progress --no-wait-for-video --no-keep-video --no-audio-multistreams --extract-audio --output ""{AppContext.BaseDirectory}Data/Music/{song.Id}"" --audio-format opus --progress-template ""download:%(progress.downloaded_bytes)s/%(progress.total_bytes)s"" https://www.youtube.com/watch?v={song.YoutubeId}",
				StandardOutputEncoding = Encoding.Unicode,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};

            if (!process.Start()) {
				return EDownloadResult.CantStart;
			}

			var sr = new StreamReader(process.StandardOutput.BaseStream);

			string[] progress;
			long bytesDownloaded = 0;
			long bytesTotal = 0;

			while (!sr.EndOfStream) {
				progress = (await sr.ReadLineAsync())?.Split('/') ?? ["0", "0"];
				if (!long.TryParse(progress[0], out bytesDownloaded)) bytesDownloaded = 0;
				if (!long.TryParse(progress[1], out bytesTotal)) bytesTotal = 0;

				_events.Broadcast(new SongDownloadProgeressUpdated() {
					SongId = song.Id,
					Progress = (double)bytesDownloaded / bytesTotal
				});
			}

			await process.WaitForExitAsync();

			var finalResult = process.ExitCode == 0 ? EDownloadResult.Success : EDownloadResult.UnknownError;

			downloadTask.SetResult(finalResult);

			_downloads.Remove(download);
			_events.Broadcast(new SongDownloadFinished() {
				SongId = song.Id,
				Result = finalResult
			});

			process.Close();
			process.Dispose();
			return finalResult;
		}
    }
}
