using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Descriptors;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class VariantsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax, 
                transform: GetDescriptor
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, (c, d) => Generate(c, d!));
    }
    
    private static VariantsDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol classType)
            return null;

        var debug = new StringBuilder();

        var methods = new Dictionary<IMethodSymbol, List<ParameterVariantsDescriptor>>(SymbolEqualityComparer.Default);
            
        foreach (var method in classType.GetMembers().OfType<IMethodSymbol>()) {
            foreach (var parameter in method.Parameters) {
                var attributes = parameter.GetAttributes()
                    .Where(a => a.AttributeClass?.Name == "VariantAttribute")
                    .ToList();
                
                var requiredAttribute = method.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "RequiredVariantAttribute");
                
                var variants = new List<VariantDescriptor?>();

                foreach (var attribute in attributes) {
                    var variantMethodName = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (variantMethodName is null) continue;
                    
                    var variantMethod = classType.GetMembers()
                        .OfType<IMethodSymbol>()
                        .FirstOrDefault(m => m.Name == variantMethodName);
                    
                    if (variantMethod is null) continue;

                    if (variantMethod.ReturnType is INamedTypeSymbol { IsTupleType: true } tupleType) {
                        foreach (var tupleElement in tupleType.TupleElements) {
                            debug.AppendLine($"Element: {tupleElement.Name}: {tupleElement.Type.GetFullName()}");
                        }
                    }
                    
                    variants.Add(new VariantDescriptor(
                        method,
                        parameter,
                        variantMethod
                    ));
                }

                debug.AppendLine($"Variants: {variants.Count}");
                
                if (variants.Any()) variants.Insert(0, null);
                else if (requiredAttribute is null) continue;

                if (!methods.ContainsKey(method)) methods[method] = [];
                
                methods[method].Add(new ParameterVariantsDescriptor(
                    method,
                    parameter,
                    variants,
                    requiredAttribute is not null
                ));
                
                debug.AppendLine($"Parameter: {method.Name} | {parameter.Name} | {string.Join(", ",
                    variants.Select(v => v is null ? "nov" : v.VariantMethod.Name)
                )}");
            }
        }
        
        if (!methods.Any()) return null;
        
        return new VariantsDescriptor(
            classType,
            methods,
            debug
        );
    }
    
    //
    //RespondOneOrManyAsync(items, oneAsync, title, getEntities)
    //RespondOneOrManyAsync(items, oneAsync, manySync, getEntities)
    //RespondOneOrManyAsync(items, oneAsync, manyAsync, getEntities)
    //RespondOneOrManyAsync(items, oneSync, title, getEntities)
    //RespondOneOrManyAsync(items, oneSync, manySync, getEntities)
    //RespondOneOrManyAsync(items, oneSync, manyAsync, getEntities)
    //

    public record FinalVariant(
        IMethodSymbol Method,
        List<ITypeParameterSymbol> TypeParameters,
        List<IParameterSymbol> InputParameters,
        List<string> OutputFlow,
        StringBuilder OutputCode
    );

    public static string? GetDefaultString(IParameterSymbol parameter) {
        var syntax = parameter.DeclaringSyntaxReferences
            .FirstOrDefault()?.GetSyntax() as ParameterSyntax;

        return syntax?.Default?.Value.ToString();
    }
    public static string FlowParameter(IParameterSymbol parameter, string? defaultString) {
        return $"{(
            parameter.IsParams ? "params " : ""
        )}{parameter.Type.GetFullName()} {parameter.Name}{(
            defaultString is not null ? $" = {defaultString}" : ""
        )}";
    }

    public static IEnumerable<FinalVariant> GetVariantsFlows(
        IMethodSymbol method,
        List<ParameterVariantsDescriptor> nextVariants,
        FinalVariant? final,
        int index
    ) {
        final ??= new FinalVariant(method, method.TypeParameters.ToList(), [], [], new StringBuilder());
        
        var currentVariants = nextVariants.FirstOrDefault();
        if (currentVariants is null) {
            for (; index < method.Parameters.Length; index++) {
                final.InputParameters.Add(method.Parameters[index]);
                final.OutputFlow.Add(method.Parameters[index].Name);
            }
            
            yield return final;
            yield break;
        }

        for (; index < method.Parameters.Length; index++) {
            var parameter = method.Parameters[index];

            if (!SymbolEqualityComparer.Default.Equals(parameter, currentVariants.Parameter)) {
                final.InputParameters.Add(parameter);
                final.OutputFlow.Add(parameter.Name);
                continue;
            }

            foreach (var variant in currentVariants.Variants) {
                var finalCopy = final with {
                    InputParameters = [..final.InputParameters],
                    OutputFlow = [..final.OutputFlow],
                    TypeParameters = [..final.TypeParameters],
                    OutputCode = new StringBuilder().Append(final.OutputCode)
                };

                var increment = 1;

                if (variant is null) {
                    finalCopy.InputParameters.Add(parameter);
                    finalCopy.OutputFlow.Add(parameter.Name);
                }
                else {
                    foreach (var typeParameter in variant.VariantMethod.TypeParameters) {
                        if (finalCopy.TypeParameters.Any(tp => tp.Name == typeParameter.Name)) continue;
                        
                        finalCopy.TypeParameters.Add(typeParameter);
                    }
                    foreach (var variantParameter in variant.VariantMethod.Parameters) {
                        if (method.Parameters.Any(p => p.Name == variantParameter.Name)) continue;
                        
                        finalCopy.InputParameters.Add(variantParameter);
                    }

                    if (variant.VariantMethod.ReturnType is INamedTypeSymbol { IsTupleType: true } tupleType) {
                        finalCopy.OutputFlow.Add(
                            string.Join(", ", tupleType.TupleElements.Select(t => t.Name))
                        );

                        finalCopy.OutputCode.AppendLine(
                            $"var ({(
                                string.Join(", ", tupleType.TupleElements.Select(t => $"{t.Name}"))
                            )}) = {variant.VariantMethod.GetFullName()}({
                                string.Join(", ", variant.VariantMethod.Parameters.Select(p => p.Name))
                            });"
                        );
                        
                        increment = tupleType.TupleElements.Length;
                    }
                    else {
                        finalCopy.OutputFlow.Add($"{variant.VariantMethod.GetFullName()}({
                            string.Join(", ", variant.VariantMethod.Parameters.Select(p => p.Name))
                        })");
                    }
                }
                
                foreach (var nextFlow in GetVariantsFlows(method, nextVariants.Skip(1).ToList(), finalCopy, index + increment)) {
                    yield return nextFlow;
                }
            }
        }
        
        /*
        sb.Append($"public ");
        sb.Append($"{method.ReturnType.GetFullName()} ");
        sb.AppendLine($"{method.Name}Variant<TType>(");
        sb.Append("    ");
        sb.AppendLine(string.Join(",\n    ", method.Parameters.Select(parameter => {
            return $"{parameter.Type.GetFullName()} {parameter.Name} {descriptor is not null}";
        })));
        sb.AppendLine(") { }");
        
        return [
            ["items", "oneAsync", "title", "getEntities"],
            ["items", "oneAsync", "manySync", "getEntities"],
            ["items", "oneAsync", "manyAsync", "getEntities"],
            ["items", "oneSync", "title", "getEntities"],
            ["items", "oneSync", "manySync", "getEntities"],
            ["items", "oneSync", "manyAsync", "getEntities"]
        ];
        */
    }

    private static string GetFinalMethodName(FinalVariant final, bool isRequired = false) {
        if (final.TypeParameters.Count == 0) return final.Method.Name;
        return $"{final.Method.Name}{(isRequired ? "Required" : "")}<{string.Join(", ", final.TypeParameters.Select(parameter => parameter.Name))}>";
    }
    private static string GetFinalMethodConstraints(FinalVariant final) {
        return string.Join(" ", final.TypeParameters.Select(GeneratorBase.GetFullyQualifiedConstraints));
    }

    private static IEnumerable<string> GetFinalMethodInputFlowReversed(FinalVariant final) {
        var foundNonDefault = false;
        foreach (var parameter in final.InputParameters
            .Distinct(SymbolEqualityComparer.Default)
            .OfType<IParameterSymbol>()
            .Reverse()
        ) {
            var defaultString = GetDefaultString(parameter);
            
            if (foundNonDefault) defaultString = null;
            else foundNonDefault = defaultString is null;
            
            yield return FlowParameter(parameter, defaultString);
        }
    }

    private static void WriteFinal(StringBuilder sb, FinalVariant final, bool isRequired = false) {
        var returnType = final.Method.ReturnType.GetFullName();
        if (isRequired && GeneratorBase.IsNullable(final.Method.ReturnType)) {
            returnType = GeneratorBase.MakeNonNullable(final.Method.ReturnType).GetFullName();
        }
        
        sb.AppendLine($"{GeneratorBase.GetMethodModifiers(final.Method)} {returnType} {GetFinalMethodName(final, isRequired)}(");
        sb.Append("    ");
        sb.AppendLine(string.Join(",\n    ", GetFinalMethodInputFlowReversed(final).Reverse().Distinct()));
        sb.AppendLine($") {GetFinalMethodConstraints(final)} {{");
        sb.AppendLine($"    {(final.OutputCode.Length > 0 ? final.OutputCode : "//no code")}");
        sb.AppendLine($"    {(final.Method.ReturnsVoid ? "" : "return ")}{final.Method.GetFullName()}(");
        sb.Append("        ");
        sb.AppendLine(string.Join(",\n        ", final.OutputFlow));
        sb.AppendLine("    );");
        sb.AppendLine("}");
    }

    private static void Generate(SourceProductionContext context, VariantsDescriptor descriptor) {
        var classNamespace = GeneratorBase.GetNamespace(descriptor.Class);
        
        var sb = new StringBuilder();
        foreach (var kvp in descriptor.ParametersWithVariants) {
            var method = kvp.Key;
            var variants = kvp.Value;
            
            var isRequired = variants.Any(v => v.IsRequired);
            
            foreach (var final in GetVariantsFlows(method, variants, null, 0).Skip(1)) {
                descriptor.DebugOutput.AppendLine($"Final: {final.Method.Name} | {final.TypeParameters.Count}");
                
                WriteFinal(sb, final);
                //if (isRequired) WriteFinal(sb, final, true);
            }
        }

        var source =
            $$"""
              namespace {{classNamespace}};
              
              {{GeneratorBase.WriteInsideClasses(
                  descriptor.Class,
                  $"",
                  sb.ToString()
              )}}
              
              /* debug output
              {{descriptor.DebugOutput}}
              */
              """;
        
        context.AddSource($"{descriptor.Class.Name}.Variants.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
