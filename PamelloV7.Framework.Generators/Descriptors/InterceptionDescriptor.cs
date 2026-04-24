using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace PamelloV7.Framework.Generators.Descriptors;

public record InterceptionDescriptor(
    IMethodSymbol Method,
    InterceptableLocation Location,
    int ParameterInsertionIndex
);
