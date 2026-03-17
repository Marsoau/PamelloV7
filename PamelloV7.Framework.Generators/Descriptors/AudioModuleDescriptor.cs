using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Descriptors;

public record AudioModuleDescriptor(
    ITypeSymbol ClassType,
    StringBuilder DebugOutput
) {
    public bool HasSingleInput;
    public bool HasSingleOutput;

    public static string GetAudioPointName() {
        return "PamelloV7.Framework.Audio.Points.AudioPoint";
    }
    
    public string GetSingleInput() {
        return $"public {GetAudioPointName()} Input => Inputs.First();";
    }

    public string GetSingleOutput() {
        return $"public {GetAudioPointName()} Output => Outputs.First();";
    }
};
