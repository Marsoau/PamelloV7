using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Downloads;

public abstract class SongDownloader
{
    protected readonly IServiceProvider _services;
    
    private readonly IEventsService _events;
    private readonly IDownloadService _downloads;
    private readonly IFileAccessService _files;

    protected readonly IDependenciesService _dependencies;
    
    private Task<EDownloadResult>? _downloadTask;

    public SongSource Source { get; init; }
    
    private int? _sourceIndex;

    private int SourceIndex
        => _sourceIndex ??= Source.Song?.Sources.ToList().IndexOf(Source) ?? -1;

    private double _progress;
    public double Progress {
        get => _progress;
        set {
            _progress = value;

            _events.Invoke(null, new SongSourceDownloadProgressUpdated() {
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
        
        _dependencies = services.GetRequiredService<IDependenciesService>();
        
        Source = source;
    }

    protected abstract Task<EDownloadResult> InternalDownloadAsync(FileInfo file);
    
    public async Task<EDownloadResult> DownloadAsync(bool forceDownload = false) {
        if (_downloadTask is not null) return await ProperDownloadReturn();
        
        var file = _files.GetSourceFile(Source);
        if (file.Exists) {
            if (!forceDownload) return EDownloadResult.Success;
            file.Delete();
        }
        
        _downloadTask = InternalDownloadAsync(file);

        try {
            return await ProperDownloadReturn();
        }
        finally {
            _downloadTask = null;
        }

        async Task<EDownloadResult> ProperDownloadReturn() {
            Debug.Assert(_downloadTask is not null);
            
            var result = EDownloadResult.UnknownError;

            try {
                result = await _downloadTask;
            }
            finally {
                _downloadTask = null;
                _downloads.RemoveDownloader(this);
            }
            
            return result;
        }
    }
}
