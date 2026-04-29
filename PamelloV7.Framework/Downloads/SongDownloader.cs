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
    
    private readonly object _taskLock = new();
    public Task<EDownloadResult>? DownloadTask;

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
    
    public EDownloadResult? Result { get; protected set; }
    
    public SongDownloader(IServiceProvider services, SongSource source) {
        _services = services;
        
        _events = services.GetRequiredService<IEventsService>();
        _downloads = services.GetRequiredService<IDownloadService>();
        _files = services.GetRequiredService<IFileAccessService>();
        
        _dependencies = services.GetRequiredService<IDependenciesService>();
        
        Source = source;
    }

    protected abstract Task<EDownloadResult> InternalDownloadAsync(FileInfo file);
    
    public Task<EDownloadResult> DownloadAsync(bool forceDownload = false) {
        lock (_taskLock) {
            if (DownloadTask is null) {
                DownloadTask = RunDownloadAsync(forceDownload);
            }
            return DownloadTask;
        }
    }

    private async Task<EDownloadResult> RunDownloadAsync(bool forceDownload) {
        try {
            var file = _files.GetSourceFile(Source);
            if (file.Exists) {
                if (!forceDownload) {
                    Result = EDownloadResult.Success;
                    return EDownloadResult.Success;
                }
                file.Delete();
            }

            Result = await InternalDownloadAsync(file);
            return Result.Value;
        }
        catch {
            Result ??= EDownloadResult.UnknownError;
            throw;
        }
        finally {
            lock (_taskLock) {
                DownloadTask = null;
            }
            _downloads.RemoveDownloader(this);
        }
    }
}
