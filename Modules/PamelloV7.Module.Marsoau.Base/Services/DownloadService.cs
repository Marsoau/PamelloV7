using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Downloads;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Services;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class DownloadService : IDownloadService
{
    private readonly IServiceProvider _services;
    
    private readonly IFileAccessService _files;
    
    private readonly List<SongDownloader> _downloaders;
    private Dictionary<string, Type> _downloaderTypes;
    
    public DownloadService(IServiceProvider services) {
        _services = services;
        
        _files = services.GetRequiredService<IFileAccessService>();
        
        _downloaders = [];
        _downloaderTypes = [];
    }

    public void Load() {
        var typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();
        
        _downloaderTypes = typeResolver.GetWithAttribute<SongDownloaderAttribute>().ToDictionary(type => type.GetCustomAttribute<SongDownloaderAttribute>()!.Name);
    }

    public bool DoesSongDownloaderExist(string platform) {
        return _downloaderTypes.ContainsKey(platform);
    }

    public SongDownloader GetSongDownloader(SongSource source) {
        var downloader = _downloaders.FirstOrDefault(downloader => downloader.Source == source);
        if (downloader is not null) return downloader;
        
        var downloaderType = _downloaderTypes.GetValueOrDefault(source.PK.Platform);
        if (downloaderType is null) throw new PamelloException($"Downloader for platform {source.PK.Platform} not found");
        
        downloader = (SongDownloader)Activator.CreateInstance(downloaderType, _services, source)!;
        _downloaders.Add(downloader);
        
        return downloader;
    }

    public void RemoveDownloader(SongDownloader downloader) {
        _downloaders.Remove(downloader);
    }
    
    public bool IsDownloading(SongSource source) {
        return _downloaders.Any(downloader => downloader.Source == source);
    }
    
    public bool IsDownloaded(SongSource source) {
        if (IsDownloading(source)) return false;
        
        var file = _files.GetSourceFile(source);
        return file.Exists;
    }
}
