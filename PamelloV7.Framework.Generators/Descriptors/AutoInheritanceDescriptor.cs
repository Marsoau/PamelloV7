using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Descriptors;

public record AutoInheritanceDescriptor(
    ITypeSymbol ClassType,
    ITypeSymbol? InheritFromType,
    ITypeSymbol[]? InterfaceTypes,
    StringBuilder DebugOutput
);
