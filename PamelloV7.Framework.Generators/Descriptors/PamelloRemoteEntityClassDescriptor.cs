
using Microsoft.CodeAnalysis;

public readonly record struct PamelloRemoteEntityClassDescriptor(
    string Namespace,
    string ClassName,
    string ProviderName,
    string RemoteInterfaceName,
    ITypeSymbol? DtoType,
    string DebugOutput = ""
);
