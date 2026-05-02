using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Scope;

public static class PamelloScope
{
    private static readonly AsyncLocal<IPamelloUser?> CurrentScopeUser = new();
    
    private static PamelloException NoUserException => new PamelloException("User is required for this operation");
    
    public static void RequireUser() {
        if (User is null) throw NoUserException;
    }

    public static IPamelloUser RequiredUser => User ?? throw NoUserException;
    public static IPamelloUser? User => CurrentScopeUser.Value;
    
    public static void SetUser(IPamelloUser? user) => CurrentScopeUser.Value = user;

    public static void SetUserIn(IPamelloUser? user, Action action) {
        var previousUser = CurrentScopeUser.Value;
        CurrentScopeUser.Value = user;

        try {
            action();
        }
        finally {
            CurrentScopeUser.Value = previousUser;
        }
    }
    
    public static T SetUserIn<T>(IPamelloUser? user, Func<T> func) {
        var previousUser = CurrentScopeUser.Value;
        CurrentScopeUser.Value = user;

        try {
            return func();
        }
        finally {
            CurrentScopeUser.Value = previousUser;
        }
    }
}
