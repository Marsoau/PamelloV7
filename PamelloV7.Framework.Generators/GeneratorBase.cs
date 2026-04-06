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
}
