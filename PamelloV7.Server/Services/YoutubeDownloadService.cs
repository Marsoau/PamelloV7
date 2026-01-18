using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Model;
using System.Diagnostics;
using System.Text;
using PamelloV7.Core.EventsOld;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Server.Config;

namespace PamelloV7.Server.Services
{
	public record YoutubeDownloadItem
	{
		public required IPamelloSong Song;
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

		private YoutubeDownloadItem? GetDownload(IPamelloSong song) {
			foreach (var download in _downloads) {
                if (download.Song == song) {
					return download;
                }
            }
			return null;
		}

		public bool IsDownloading(IPamelloSong song) {
			return GetDownload(song) is not null;
		}
		public double GetProgress(IPamelloSong song) {
			var download = GetDownload(song);
			return download?.Progress ?? 0;
		}

		public Task<EDownloadResult> DownloadFromYoutubeAsync(IPamelloSong song, bool forceDownload = false) {
			var download = GetDownload(song);
			if (download is not null) {
				return download.Task;
			}
			if (true) { //if (song.SelectedSource.IsDownloaded) {
				if (!forceDownload) return Task.FromResult(EDownloadResult.Success);

				if (File.Exists($@"{PamelloServerConfig.Root.DataPath}/Music/{song.Id}.opus"))
					File.Delete($@"{PamelloServerConfig.Root.DataPath}/Music/{song.Id}.opus");
			}

			return Task.Run(() => DownloadFromYoutube(song, forceDownload));
		}
		public EDownloadResult DownloadFromYoutube(IPamelloSong song, bool forceDownload = false) {
			if (true) { //if (song.SelectedSource.IsDownloaded) {
				if (!forceDownload) return EDownloadResult.Success;

				if (File.Exists($@"{PamelloServerConfig.Root.DataPath}/Music/{song.Id}.opus"))
					File.Delete($@"{PamelloServerConfig.Root.DataPath}/Music/{song.Id}.opus");
			}
			
			var downloadTask = new TaskCompletionSource<EDownloadResult>();

			var download = new YoutubeDownloadItem() {
				Song = song,
				Task = downloadTask.Task,
				Progress = 0
			};

			_downloads.Add(download);
			_events.Broadcast(new SongDownloadStarted() {
				SongId = song.Id,
			});

			if (!Directory.Exists($"{PamelloServerConfig.Root.DataPath}/Music")) {
				Directory.CreateDirectory($"{PamelloServerConfig.Root.DataPath}/Music");
			}

			using var process = new Process();
			process.StartInfo = new ProcessStartInfo() {
				FileName = $@"yt-dlp",
				Arguments = $@"--quiet --newline --progress --no-wait-for-video --no-keep-video --no-audio-multistreams --extract-audio --output ""{PamelloServerConfig.Root.DataPath}/Music/{song.Id}"" --audio-format opus --progress-template ""download:%(progress.downloaded_bytes)s/%(progress.total_bytes)s"" https://www.youtube.com/watch?v=song.YoutubeId",
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
				progress = (sr.ReadLine())?.Split('/') ?? ["0", "0"];
				if (!long.TryParse(progress[0], out bytesDownloaded)) bytesDownloaded = 0;
				if (!long.TryParse(progress[1], out bytesTotal)) bytesTotal = 0;

				_events.Broadcast(new SongDownloadProgeressUpdated() {
					SongId = song.Id,
					Progress = (double)bytesDownloaded / bytesTotal
				});
			}

			process.WaitForExit();

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
