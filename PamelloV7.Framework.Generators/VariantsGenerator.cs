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
                
                if (!attributes.Any()) continue;

                var variants = new List<VariantDescriptor?>();

                foreach (var attribute in attributes) {
                    var variantMethodName = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (variantMethodName is null) continue;
                    
                    var variantMethod = classType.GetMembers()
                        .OfType<IMethodSymbol>()
                        .FirstOrDefault(m => m.Name == variantMethodName);
                    
                    if (variantMethod is null) continue;
                    
                    variants.Add(new VariantDescriptor(
                        method,
                        parameter,
                        variantMethod
                    ));
                }

                debug.AppendLine($"Variants: {variants.Count}");
                
                if (!variants.Any()) continue;
                
                variants.Insert(0, null);

                if (!methods.ContainsKey(method)) methods[method] = [];
                methods[method].Add(new ParameterVariantsDescriptor(
                    method,
                    parameter,
                    variants
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
        List<string> InputFlow,
        List<string> OutputFlow
    );

    public static string FlowParameter(IParameterSymbol parameter) {
        var syntax = parameter.DeclaringSyntaxReferences
            .FirstOrDefault()?.GetSyntax() as ParameterSyntax;

        var defaultString = syntax?.Default?.Value.ToString();
        
        return $"{parameter.Type.GetFullName()} {parameter.Name}{(defaultString is not null ? $" = {defaultString}" : "")}";
    }

    public static IEnumerable<FinalVariant> GetVariantsFlows(
        IMethodSymbol method,
        List<ParameterVariantsDescriptor> nextVariants,
        FinalVariant? final,
        int index
    ) {
        final ??= new FinalVariant(method, [], []);
        
        var currentVariants = nextVariants.FirstOrDefault();
        if (currentVariants is null) {
            for (; index < method.Parameters.Length; index++) {
                final.InputFlow.Add(FlowParameter(method.Parameters[index]));
                final.OutputFlow.Add(method.Parameters[index].Name);
            }
            
            yield return final;
            yield break;
        }

        for (; index < method.Parameters.Length; index++) {
            var parameter = method.Parameters[index];

            if (!SymbolEqualityComparer.Default.Equals(parameter, currentVariants.Parameter)) {
                final.InputFlow.Add(FlowParameter(parameter));
                final.OutputFlow.Add(parameter.Name);
                continue;
            }

            foreach (var variant in currentVariants.Variants) {
                var finalCopy = final with {
                    InputFlow = [..final.InputFlow],
                    OutputFlow = [..final.OutputFlow]
                };

                if (variant is null) {
                    finalCopy.InputFlow.Add(FlowParameter(parameter));
                    finalCopy.OutputFlow.Add(parameter.Name);
                }
                else {
                    var outputMethodBuilder = new StringBuilder();
                    
                    outputMethodBuilder.Append($"{variant.VariantMethod.GetFullName()}(");

                    foreach (var variantParameter in variant.VariantMethod.Parameters) {
                        if (method.Parameters.Any(p => p.Name == variantParameter.Name)) continue;
                        
                        finalCopy.InputFlow.Add(FlowParameter(variantParameter));
                    }
                    
                    outputMethodBuilder.Append(string.Join(", ", variant.VariantMethod.Parameters.Select(p => p.Name)));

                    outputMethodBuilder.Append(')');
                    
                    finalCopy.OutputFlow.Add(outputMethodBuilder.ToString());
                }
                
                foreach (var nextFlow in GetVariantsFlows(method, nextVariants.Skip(1).ToList(), finalCopy, index + 1)) {
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

    private static void Generate(SourceProductionContext context, VariantsDescriptor descriptor) {
        var classNamespace = GeneratorBase.GetNamespace(descriptor.Class);
        
        var sb = new StringBuilder();
        foreach (var kvp in descriptor.ParametersWithVariants) {
            var method = kvp.Key;
            var variants = kvp.Value;
            
            foreach (var final in GetVariantsFlows(method, variants, null, 0).Skip(1)) {
                sb.AppendLine($"{GeneratorBase.GetMethodModifiers(final.Method)} {final.Method.ReturnType.GetFullName()} {final.Method.GetFullName()}(");
                sb.Append("    ");
                sb.AppendLine(string.Join(",\n    ", final.InputFlow));
                sb.AppendLine($") {GeneratorBase.GetFullyQualifiedConstraints(final.Method)} {{");
                sb.AppendLine($"    return {final.Method.GetFullName()}(");
                sb.Append("        ");
                sb.AppendLine(string.Join(",\n        ", final.OutputFlow));
                sb.AppendLine("    );");
                sb.AppendLine("}");
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
              u
              */
              """;
        
        context.AddSource($"{descriptor.Class.Name}.Variants.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
