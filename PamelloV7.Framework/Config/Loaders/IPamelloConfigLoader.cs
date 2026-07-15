using PamelloV7.Framework.Config.Parts;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Config.Loaders;

public interface IPamelloConfigLoader
{
    public static string DefaultDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PamelloV7"
    );
    
    public static string DefaultConfigFilePath => Path.Combine(DefaultDataPath, "Config", "config.jsonc");
    public static string DefaultConfigExampleFilePath => Path.Combine(DefaultDataPath, "Config", "configEXAMPLE.jsonc");
    
    public List<PamelloConfigPart> Parts { get; }
}
