using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class DownloadService : IDownloadService
{
    private readonly IServiceProvider _services;
    
    private readonly IFileAccessService _files;
    
    private readonly ConcurrentDictionary<SongSource, SongDownloader> _downloaders;
    
    private Dictionary<string, Type> _downloaderTypes;
    
    public DownloadService(IServiceProvider services) {
        _services = services;
        
        _files = services.GetRequiredService<IFileAccessService>();
        
        _downloaders = [];
        _downloaderTypes = [];
    }

    public void Startup(IServiceProvider services) {
        var typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();
        
        _downloaderTypes = typeResolver.GetWithAttribute<SongDownloaderAttribute>().ToDictionary(type => type.GetCustomAttribute<SongDownloaderAttribute>()!.Name);
    }

    public bool DoesSongDownloaderExist(string platform) {
        return _downloaderTypes.ContainsKey(platform);
    }
    
    public SongDownloader GetSongDownloaderRequired(SongSource source)
        => GetSongDownloader(source) ?? throw new PamelloException($"Downloader for {source.PK.Platform} not found");

    public SongDownloader? GetSongDownloader(SongSource source) {
        var downloaderType = _downloaderTypes.GetValueOrDefault(source.PK.Platform);
        if (downloaderType is null) return null;
        
        return _downloaders.GetOrAdd(source, s =>
            (SongDownloader)Activator.CreateInstance(downloaderType, _services, s)!
        );
    }

    public void RemoveDownloader(SongDownloader downloader) {
        _downloaders.Remove(downloader.Source, out _);
    }
    
    public bool IsDownloading(SongSource? source) {
        if (source is null) return false;
        
        return _downloaders.GetValueOrDefault(source) is {
            DownloadTask: not null,
            Result: null
        };
    }
    
    public bool IsDownloaded(SongSource? source) {
        Output.Write($"IsISnss: {IsDownloading(source)}");
        if (IsDownloading(source) || source is null) return false;
        
        var file = _files.GetSourceFile(source);
        return file.Exists;
    }
}
