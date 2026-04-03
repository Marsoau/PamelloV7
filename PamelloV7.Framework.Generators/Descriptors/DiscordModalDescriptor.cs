using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Descriptors;

public record DiscordModalPropertyDescriptor(
    ITypeSymbol PropertiesType,
    ITypeSymbol ValueType,
    string Name
);

public record DiscordModalDescriptor(
    ITypeSymbol ModalClass,
    ITypeSymbol? BuilderClass,
    DiscordModalPropertyDescriptor[] Properties,
    StringBuilder DebugOutput
);
