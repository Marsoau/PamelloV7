
using Microsoft.CodeAnalysis;

public enum ECategory
{
    Class,
    Record,
}

public readonly record struct SafeStoredEntitiesClassDescriptor(
    string Namespace,
    string ClassName,
    ECategory Category,
    List<SafeStoredEntityDescriptor> SingleEntitiesInfos,
    List<SafeStoredEntityDescriptor> ManyEntitiesInfos,
    string DebugOutput = ""
);

public readonly record struct SafeStoredEntityDescriptor(
    ITypeSymbol EntityType,
    string PropertyName,
    string[] PropertyAttributes,
    bool IsRequired
);
