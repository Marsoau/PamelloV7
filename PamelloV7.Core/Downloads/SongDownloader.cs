using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Core.Services;

namespace PamelloV7.Core.Downloads;

public abstract class SongDownloader
{
    private readonly IServiceProvider _services;
    
    private readonly IEventsService _events;
    private readonly IDownloadService _downloads;
    private readonly IFileAccessService _files;
    
    private Task<EDownloadResult>? _downloadTask;

    public SongSource Source { get; init; }
    
    private int? _sourceIndex;

    private int SourceIndex
        => _sourceIndex ??= Source.Song.Sources.ToList().IndexOf(Source);

    private double _progress;
    public double Progress {
        get => _progress;
        set {
            _progress = value;

            _events.Invoke(new SongSourceDownloadProgressUpdated() {
                Song = Source.Song,
                SourceIndex = SourceIndex,
                Progress = _progress
            });
        }
    }
    
    public SongDownloader(IServiceProvider services, SongSource source) {
        _services = services;
        
        _events = services.GetRequiredService<IEventsService>();
        _downloads = services.GetRequiredService<IDownloadService>();
        _files = services.GetRequiredService<IFileAccessService>();
        
        Source = source;
    }

    protected abstract Task<EDownloadResult> InternalDownloadAsync(FileInfo file);
    
    public async Task<EDownloadResult> DownloadAsync(bool forceDownload = false) {
        if (_downloadTask is not null) return await _downloadTask;
        
        var file = _files.GetSourceFile(Source);
        if (file.Exists) {
            if (!forceDownload) return EDownloadResult.Success;
            file.Delete();
        }
        
        _downloadTask = InternalDownloadAsync(file);

        try {
            return await _downloadTask;
        }
        finally {
            _downloadTask = null;
            _downloads.RemoveDownloader(this);
        }
    }
}
