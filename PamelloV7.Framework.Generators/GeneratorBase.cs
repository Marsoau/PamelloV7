using System.Text;
using Microsoft.CodeAnalysis;

namespace PamelloV7.Framework.Generators;

public static class GeneratorBase
{
    public static string Tab(int count) => string.Join("", Enumerable.Repeat("    ", count));
    
    public static string GetNamespace(ITypeSymbol classSymbol) {
        return classSymbol.ContainingNamespace.IsGlobalNamespace 
            ? string.Empty 
            : classSymbol.ContainingNamespace.ToDisplayString();
    }

    public static IEnumerable<string> GetContainingTypes(ITypeSymbol classSymbol) {
        while (true) {
            var containingType = classSymbol.ContainingType;
            if (containingType is null) yield break;
            
            yield return containingType.Name;

            classSymbol = containingType;
        }
    }

    public static void WriteInsideClasses(string[] classNames, string inheritancePart, string innerPart, StringBuilder sb, int depth = 0) {
        if (classNames.Length == 0) return;
        
        sb.Append($"{Tab(depth)}public partial class {classNames[depth]} ");
        
        if (depth + 1 >= classNames.Length) {
            sb.AppendLine($"{inheritancePart}{(
                inheritancePart.LastOrDefault() == ' ' ? "{" : " {"
            )}");

            sb.Append(Tab(depth + 1));
            sb.AppendLine(innerPart.Replace("\n", $"\n{Tab(depth + 1)}"));
            
            sb.AppendLine($"{Tab(depth)}}}");
            return;
        }

        sb.AppendLine($"{{");
        WriteInsideClasses(classNames, inheritancePart, innerPart, sb, depth + 1);
        sb.AppendLine($"{Tab(depth)}}}");
    }
    
    public static StringBuilder WriteInsideClasses(ITypeSymbol type, string inheritancePart, string innerPart) {
        var sb = new StringBuilder();
        
        var containingTypes = GetContainingTypes(type).ToArray();
        WriteInsideClasses([..containingTypes, type.Name], inheritancePart, innerPart, sb);
        
        return sb;
    }
    
    public static bool HasMethod(ITypeSymbol? type, string methodName, StringBuilder? debug = null) {
        while (true) {
            if (type is null) return false;

            debug?.AppendLine($"Checking: {type.Name}");
            foreach (var member in type.GetMembers().OfType<IMethodSymbol>()) {
                debug?.AppendLine($"| {member.Name}");
            }
            
            if (type.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == methodName))
                return true;

            type = type.BaseType;
        }
    }

    public static string GetFullyQualifiedConstraints(IMethodSymbol methodSymbol) {
        if (methodSymbol.TypeParameters.Length == 0)
            return string.Empty;

        var constraints = methodSymbol.TypeParameters.Select(GetFullyQualifiedConstraints);
        return string.Join(" ", constraints);
    }
    public static string GetFullyQualifiedConstraints(ITypeParameterSymbol typeParam) {
        var constraints = new List<string>();

        if (typeParam.HasReferenceTypeConstraint) {
            constraints.Add(typeParam.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
        }
        else if (typeParam.HasValueTypeConstraint) {
            constraints.Add("struct");
        }
        else if (typeParam.HasUnmanagedTypeConstraint) {
            constraints.Add("unmanaged");
        }
        else if (typeParam.HasNotNullConstraint) {
            constraints.Add("notnull");
        }

        foreach (ITypeSymbol typeConstraint in typeParam.ConstraintTypes) {
            constraints.Add(typeConstraint.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        if (typeParam.HasConstructorConstraint) {
            constraints.Add("new()");
        }
        if (constraints.Count > 0) {
            return $"where {typeParam.Name} : {string.Join(", ", constraints)}";
        }

        return "";
    }
    
    public static string GetMethodModifiers(IMethodSymbol methodSymbol) {
        var modifiers = new List<string>();

        var accessibility = methodSymbol.DeclaredAccessibility switch {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Private => "private",
            _ => ""
        };

        if (!string.IsNullOrEmpty(accessibility))
            modifiers.Add(accessibility);
        if (methodSymbol.IsStatic)
            modifiers.Add("static");
        if (methodSymbol.IsAbstract)
            modifiers.Add("abstract");
        if (methodSymbol.IsVirtual)
            modifiers.Add("virtual");
        if (methodSymbol.IsOverride)
            modifiers.Add("override");
        if (methodSymbol.IsExtern)
            modifiers.Add("extern");

        return string.Join(" ", modifiers);
    }
    
    /// <summary>
    /// Checks if a type is nullable, either directly (e.g. string?) or inside a Task (e.g. Task<string?>).
    /// </summary>
    public static bool IsNullable(ITypeSymbol typeSymbol)
    {
        // 1. If it's a Task<T>, check the inner T
        if (IsGenericTask(typeSymbol, out ITypeSymbol innerType))
        {
            // Return true if the inner type is nullable OR if the Task itself is nullable (Task<T>?)
            return IsTypeDirectlyNullable(innerType) || IsTypeDirectlyNullable(typeSymbol);
        }

        // 2. Check the type itself
        return IsTypeDirectlyNullable(typeSymbol);
    }

    /// <summary>
    /// Returns a non-nullable version of the type. 
    /// If the type is a Task<T?>, it reconstructs and returns Task<T>.
    /// </summary>
    public static ITypeSymbol MakeNonNullable(ITypeSymbol typeSymbol)
    {
        // 1. If it's a Task<T>, reconstruct it with a non-nullable T
        if (IsGenericTask(typeSymbol, out ITypeSymbol innerType))
        {
            // Make the inner type non-nullable
            ITypeSymbol nonNullableInner = MakeTypeDirectlyNonNullable(innerType);

            // Get the original unbound generic definition (Task<>)
            INamedTypeSymbol namedType = (INamedTypeSymbol)typeSymbol;
            INamedTypeSymbol unboundTaskType = namedType.OriginalDefinition;

            // Reconstruct the Task with the new non-nullable inner type -> Task<T>
            ITypeSymbol reconstructedTask = unboundTaskType.Construct(nonNullableInner);

            // Ensure the Task itself isn't nullable (Task?) and return it
            return MakeTypeDirectlyNonNullable(reconstructedTask);
        }

        // 2. Make the type itself non-nullable
        return MakeTypeDirectlyNonNullable(typeSymbol);
    }

    // --- Private Internal Helpers ---

    private static bool IsGenericTask(ITypeSymbol typeSymbol, out ITypeSymbol innerType)
    {
        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            string ns = namedType.ContainingNamespace?.ToDisplayString();
            string name = namedType.MetadataName; // MetadataName includes the tick, e.g. "Task`1"

            if (ns == "System.Threading.Tasks" && (name == "Task`1" || name == "ValueTask`1"))
            {
                innerType = namedType.TypeArguments[0];
                return true;
            }
        }

        innerType = null!;
        return false;
    }

    private static bool IsTypeDirectlyNullable(ITypeSymbol type)
    {
        // Check Value Types (int?)
        if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T) return true;
        
        // Check Reference Types (string?)
        if (type.NullableAnnotation == NullableAnnotation.Annotated) return true;

        return false;
    }

    private static ITypeSymbol MakeTypeDirectlyNonNullable(ITypeSymbol type)
    {
        // Strip Value Type Nullability (Nullable<T> -> T)
        if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && type is INamedTypeSymbol namedType)
        {
            return namedType.TypeArguments[0];
        }

        // Strip Reference Type Nullability (string? -> string)
        return type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
    }
}
