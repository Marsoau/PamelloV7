using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Config.Loaders;
using PamelloV7.Framework.Dependencies;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Services;

namespace PamelloV7.Server.Services;

public class FileAccessService : IFileAccessService
{
    private readonly IServiceProvider _services;
    
    private readonly DirectoryInfo RootDirectory;
    
    private readonly DirectoryInfo FilesDirectory;
    private readonly DirectoryInfo AudioDirectory;

    public FileAccessService(IServiceProvider services) {
        _services = services;
        
        RootDirectory = new DirectoryInfo(IPamelloConfigLoader.DefaultDataPath);
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
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) {
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
            else if (OperatingSystem.IsMacOS()) {
                extension = ".dylib";
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
        //
        //"lite-old70.db" is an outdated database file name from 2.0.1, later should be handled by migrations properly, but for now it is how it is
        //
        
        var outdatedDataFile = new FileInfo(Path.Combine(RootDirectory.FullName, "lite-old70.db"));
        var outdatedLogFile = new FileInfo(Path.Combine(RootDirectory.FullName, "lite-old70-log.db"));
        
        var dataFile = new FileInfo(Path.Combine(RootDirectory.FullName, "data.db"));
        var logFile = new FileInfo(Path.Combine(RootDirectory.FullName, "data-log.db"));

        dataFile = outdatedDataFile.Exists && !dataFile.Exists
            ? outdatedDataFile.CopyTo(Path.Combine(RootDirectory.FullName, "data.db"), true)
            : dataFile;
        logFile = outdatedLogFile.Exists && !logFile.Exists
            ? outdatedLogFile.CopyTo(Path.Combine(RootDirectory.FullName, "data-log.db"), true)
            : logFile;

        if (!ServerConfig.Root.DoNotSaveChanges) return dataFile;
        
        var tempDatabaseFile = dataFile.Exists
            ? dataFile.CopyTo(Path.Combine(RootDirectory.FullName, "data-TEMP.db"), true)
            : new FileInfo(Path.Combine(RootDirectory.FullName, "data-TEMP.db"));
        
        if (!logFile.Exists) logFile.CopyTo(Path.Combine(RootDirectory.FullName, "data-TEMP-log.db"), true);
            
        return tempDatabaseFile;
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
