using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Extensions;

public static class TypeSymbolExtensions
{
    public static string GetFullName(this ITypeSymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}
