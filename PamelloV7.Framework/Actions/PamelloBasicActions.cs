using System.Reflection;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Actions;

public class PamelloBasicActions
{
    public static async Task<object?> RunExecuteMethodAsync(object? obj, object?[]? arguments = null, Action<Exception>? exceptionHandler = null) {
        if (obj is null) return null;
        
        var method = obj.GetType().GetMethod("Execute");
        if (method is null) throw new PamelloException("Could not find Execute method");
        
        return await RunMethodAsync(method, obj, arguments, exceptionHandler);
    }

    public static async Task<object?> RunMethodAsync(MethodInfo method, object obj, object?[]? arguments = null, Action<Exception>? exceptionHandler = null) {
        exceptionHandler ??= x => throw x;

        try {
            object? result;
            
            if (typeof(Task).IsAssignableFrom(method.ReturnType)) {
                if (method.ReturnType.IsGenericType) {
                    result = await (dynamic)method.Invoke(obj, arguments ?? [])!;
                }
                else {
                    await (Task)method.Invoke(obj, arguments ?? [])!;
                    result = null;
                }
            }
            else {
                result = method.Invoke(obj, arguments ?? []);
            }
            
            return result;
        }
        catch (Exception x) {
            exceptionHandler(x);
            return null;
        }
    }
}
