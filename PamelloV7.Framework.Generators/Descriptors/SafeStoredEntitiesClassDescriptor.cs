
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
    string DebugOutput = ""
);

public readonly record struct SafeStoredEntityDescriptor(
    ITypeSymbol EntityType,
    string PropertyName,
    string[] PropertyAttributes
);
