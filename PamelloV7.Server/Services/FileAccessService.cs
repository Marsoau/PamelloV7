using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Services;

namespace PamelloV7.Server.Services;

public class FileAccessService : IFileAccessService
{
    private readonly IServiceProvider _services;
    
    public static DirectoryInfo ConfigDirectory => new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Config"));
    
    private readonly DirectoryInfo RootDirectory;
    
    private readonly DirectoryInfo FilesDirectory;
    private readonly DirectoryInfo AudioDirectory;

    public FileAccessService(IServiceProvider services) {
        _services = services;
        
        RootDirectory = new DirectoryInfo(ServerConfig.Root.DataPath);
        FilesDirectory = new DirectoryInfo(Path.Combine(RootDirectory.FullName, "Files"));
        AudioDirectory = new DirectoryInfo(Path.Combine(FilesDirectory.FullName, "Audio"));
    }

    public void Startup(IServiceProvider services) {
        Directory.CreateDirectory(RootDirectory.FullName);
        FilesDirectory.Create();
        AudioDirectory.Create();
    }

    public FileInfo GetFileRequired(string requestedPath)
        => GetFile(requestedPath) ?? throw new PamelloException("Unreacheable file");
    public FileInfo? GetFile(string requestedPath) {
        if (requestedPath.Split('/').Any(part => part == "..")) {
            return null;
        }
        
        return new FileInfo(
            Path.Combine(FilesDirectory.FullName, requestedPath)
        );
    }

    public FileInfo GetSourceFile(SongSource source) {
        return new FileInfo(
            Path.Combine(AudioDirectory.FullName, $"{source._safeSong.Id}-{source.PK}.opus")
        );
    }

    public FileInfo GetDependencyFile(Dependency dependency) {
        var dependencyDirectory = GetDependencyDirectory(dependency);

        string extension;
        if (dependency.IsExecutable) {
            if (OperatingSystem.IsWindows()) {
                extension = ".exe";
            }
            else if (OperatingSystem.IsLinux()) {
                extension = "";
            }
            else {
                throw new PamelloException("Unsupported operating system");
            }
        }
        else {
            if (OperatingSystem.IsWindows()) {
                extension = ".dll";
            }
            else if (OperatingSystem.IsLinux()) {
                extension = ".so";
            }
            else {
                throw new PamelloException("Unsupported operating system");
            }
        }
        
        return new FileInfo(
            Path.Combine(dependencyDirectory.FullName, $"{dependency.InternalFilePath}{extension}")
        );
    }

    public FileInfo GetDatabaseFile() {
        return new FileInfo(
            Path.Combine(RootDirectory.FullName, "lite-old70.db")
        );
    }

    public DirectoryInfo GetDependencyDirectory(Dependency dependency) {
        var dependenciesDirectory = new DirectoryInfo(
            Path.Combine(AppContext.BaseDirectory, "Dependencies")
        );
        if (!dependenciesDirectory.Exists) dependenciesDirectory.Create();
        
        var dependencyDirectory = new DirectoryInfo(
            Path.Combine(dependenciesDirectory.FullName, dependency.Name)
        );
        if (!dependencyDirectory.Exists) dependencyDirectory.Create();
        
        return dependencyDirectory;
    }

    public string GetPublicUrl(string path) {
        return $"https://{ServerConfig.Root.HostName}/Files{path}";
    }
}
