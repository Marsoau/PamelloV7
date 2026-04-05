using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators.Descriptors;

public record VariantDescriptor(
    IMethodSymbol ParentMethod,
    IParameterSymbol Parameter,
    IMethodSymbol VariantMethod
);

public record ParameterVariantsDescriptor(
    IMethodSymbol Method,
    IParameterSymbol Parameter,
    List<VariantDescriptor?> Variants
);

public record VariantsDescriptor(
    ITypeSymbol Class,
    Dictionary<IMethodSymbol, List<ParameterVariantsDescriptor>> ParametersWithVariants,
    StringBuilder DebugOutput
);
