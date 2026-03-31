using PamelloV7.Framework.Config.Attributes;
using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Module.Marsoau.Test.Config;

[ConfigRoot]
public class TestNode
{
    public required int TtVl { get; set; }
    public required string TStr { get; set; }
    public required ELoadingStage TStage { get; set; }
    public required bool IsGay { get; set; }
}
