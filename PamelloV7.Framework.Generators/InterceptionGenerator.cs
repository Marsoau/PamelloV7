using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PamelloV7.Framework.Generators.Descriptors;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class InterceptionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is InvocationExpressionSyntax, 
                transform: GetDescriptor
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, (c, d) => Generate(c, d!));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "InterceptsLocationAttribute.g.cs",
            SourceText.From(
                """
                namespace System.Runtime.CompilerServices
                {
                    [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
                    internal sealed class InterceptsLocationAttribute : global::System.Attribute
                    {
                        public InterceptsLocationAttribute(int version, string data) { }
                    }
                }
                """, Encoding.UTF8)));
    }
    
    private static InterceptionDescriptor? GetDescriptor(GeneratorSyntaxContext context, CancellationToken ct) {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (method is null) return null;

        var attribute = method.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "InterceptedParameterInsertionAttribute");

        var index = attribute?.ConstructorArguments.ElementAtOrDefault(0).Value as int?;
        if (index is null) return null;
        
        if (!method.GetAttributes().Any()) return null;
        
        var location = context.SemanticModel.GetInterceptableLocation(invocation);
        if (location is null) return null;
        
        return new InterceptionDescriptor(method, location, index.Value);
    }

    private static void Generate(SourceProductionContext context, InterceptionDescriptor descriptor) {
        var source =
            $$"""
            namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;
            
            file static class Interceptors_{{(uint)descriptor.Location.Data.GetHashCode()}}
            {
                {{descriptor.Location.GetInterceptsLocationAttributeSyntax()}}
                public static void {{descriptor.Method.Name}}(this Ping ping) {
                    PamelloV7.Framework.Logging.Output.Write("Intercepted at {{descriptor.ParameterInsertionIndex}}");
                    ping.TestInterception();
                    ping.TestInterception();
                    ping.TestInterception();
                }
            }
            """;
        context.AddSource(
            $"{descriptor.Method.Name}_{descriptor.Location.GetHashCode():x}.Interceptions.g.cs",
            SourceText.From(source, Encoding.UTF8));
    }
}
