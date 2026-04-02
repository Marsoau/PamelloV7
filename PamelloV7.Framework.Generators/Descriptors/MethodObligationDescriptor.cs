using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Descriptors;

public record MethodObligationDescriptor(
    ITypeSymbol ClassType,
    string[] RequiredMethods,
    StringBuilder DebugOutput
);
