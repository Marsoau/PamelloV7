using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;

namespace PamelloV7.Server.Loaders.Context;

public class PamelloModuleLoadContext : AssemblyLoadContext
{
    private readonly Dictionary<string, byte[]> _dllBytesCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, byte[]> _pdbBytesCache = new(StringComparer.OrdinalIgnoreCase);
    
    private readonly Dictionary<string, Assembly> _loadedAssemblies = new(StringComparer.OrdinalIgnoreCase);

    public PamelloModuleLoadContext() : base(isCollectible: true) { }

    public void PreloadFileToMemory(string pv7mFilePath)
    {
        using var archive = ZipFile.OpenRead(pv7mFilePath);
        
        foreach (var entry in archive.Entries)
        {
            var name = Path.GetFileNameWithoutExtension(entry.Name);
            
            if (entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                if (!_dllBytesCache.ContainsKey(name))
                {
                    _dllBytesCache[name] = ReadFully(entry);
                }
            }
            else if (entry.Name.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase))
            {
                if (!_pdbBytesCache.ContainsKey(name))
                {
                    _pdbBytesCache[name] = ReadFully(entry);
                }
            }
        }
    }

    private static byte[] ReadFully(ZipArchiveEntry entry)
    {
        using var stream = entry.Open();
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var name = assemblyName.Name!;
        
        if (_loadedAssemblies.TryGetValue(name, out var cachedAssembly)) {
            return cachedAssembly;
        }

        try {
            var defaultAssembly = Default.LoadFromAssemblyName(assemblyName);
            if (defaultAssembly != null) return null;
        }
        catch { /* ignored */ }

        if (!_dllBytesCache.TryGetValue(name, out var dllBytes)) return null;
        
        Assembly assembly;
        
        using var dllStream = new MemoryStream(dllBytes);

        if (_pdbBytesCache.TryGetValue(name, out var pdbBytes)) {
            using var pdbStream = new MemoryStream(pdbBytes);
            assembly = LoadFromStream(dllStream, pdbStream);
        }
        else {
            assembly = LoadFromStream(dllStream);
        }

        _loadedAssemblies[name] = assembly;
        
        return assembly;
    }

    public Assembly LoadMainModule(string moduleName)
    {
        return Load(new AssemblyName(moduleName)) 
            ?? throw new FileNotFoundException($"Could not find module DLL in archives: {moduleName}");
    }
}