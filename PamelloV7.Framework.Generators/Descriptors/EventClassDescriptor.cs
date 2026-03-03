
using System.Text;
using Microsoft.CodeAnalysis;

public readonly record struct EventClassDescriptor(
    string Namespace,
    string ClassName,
    bool NeedsInvoker,
    bool NeedsRevertPack,
    bool HasDefaultPack,
    EventClassInfoUpdateEntry[] UpdateEntries,
    StringBuilder DebugOutput
);

public readonly record struct EventClassInfoUpdateEntry(
    ITypeSymbol DtoType,
    string EntityPropertyName,
    string[] UpdatePropertyPath
);
