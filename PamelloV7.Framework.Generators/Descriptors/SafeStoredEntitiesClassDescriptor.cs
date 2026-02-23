
using Microsoft.CodeAnalysis;

public readonly record struct SafeStoredEntitiesClassDescriptor(
    string Namespace,
    string ClassName,
    List<SafeStoredEntityDescriptor> EntityInfos
);

public readonly record struct SafeStoredEntityDescriptor(
    ITypeSymbol EntityType,
    string PropertyName
);
