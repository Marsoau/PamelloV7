using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using PamelloV7.Framework.Generators.Extensions;

namespace PamelloV7.Framework.Generators;

[Generator]
public class PamelloWrapperEntitiesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValuesProvider<PamelloRemoteEntityClassDescriptor> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: GetDescriptor
            ).Where(descriptor => descriptor is not { DtoType: null });
        
        context.RegisterSourceOutput(classDeclarations, ExecuteGeneration);
    }

    private static PamelloRemoteEntityClassDescriptor GetDescriptor(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        var classDeclaration = context.Node;
        
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken) is not INamedTypeSymbol classSymbol) {
            return default;
        }
        
        var attribute = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "RemoteEntityAttribute");
        if (attribute is null) return default;
        
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null) return default;
        
        var providerName = attribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
        var remoteInterfaceName = attribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();
        var dtoType = attributeClass.TypeArguments.FirstOrDefault();
        
        if (string.IsNullOrEmpty(providerName) || string.IsNullOrEmpty(remoteInterfaceName) || dtoType is null) return default;

        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classSymbol.ContainingNamespace.ToDisplayString();
        
        return new PamelloRemoteEntityClassDescriptor(
            namespaceName,
            classSymbol.Name,
            providerName!,
            remoteInterfaceName!,
            dtoType,
            ""
        );
    }

    private static void ExecuteGeneration(SourceProductionContext context, PamelloRemoteEntityClassDescriptor descriptor) {
        if (descriptor.DtoType is null) return;
        
        var propertiesSb = new StringBuilder();

        foreach (var propertySymbol in descriptor.DtoType.GetMembers().OfType<IPropertySymbol>().Where(symbol => !symbol.IsImplicitlyDeclared)) {
            propertiesSb.AppendLine($"        public {propertySymbol.Type.GetFullName()} {propertySymbol.Name} => Dto.{propertySymbol.Name};");
        }
        
        var source = 
            $$"""
              //auto generated
              
              //test
              
              //{{descriptor.ProviderName}}
              //{{descriptor.RemoteInterfaceName}}
              //{{descriptor.DtoType.Name}}
              
              using PamelloV7.Wrapper.Entities.Base;
              
              namespace {{descriptor.Namespace}}
              {
                  partial class {{descriptor.ClassName}} : PamelloEntity<{{descriptor.DtoType.GetFullName()}}>
                  {
              {{propertiesSb}}
                      public {{descriptor.ClassName}}({{descriptor.DtoType.GetFullName()}} dto) : base(dto) { }
                  }
              }
              """;
        context.AddSource($"{descriptor.ClassName}.Entities.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
