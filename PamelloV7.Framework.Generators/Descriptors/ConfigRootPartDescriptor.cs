using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Descriptors;

public record ConfigRootPartDescriptor(
    ITypeSymbol ClassType,
    StringBuilder DebugOutput
);
