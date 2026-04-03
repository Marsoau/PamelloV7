using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Descriptors;

public enum EDiscordModalFieldType {
    TextInput
}

public record DiscordModalFieldDescriptor(
    DiscordModalDescriptor Parent,
    EDiscordModalFieldType Type,
    string Name
);

public record DiscordModalDescriptor(
    ITypeSymbol ModalClass,
    ITypeSymbol? BuilderClass,
    DiscordModalFieldDescriptor[] Fields,
    StringBuilder DebugOutput
);
