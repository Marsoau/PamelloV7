using PamelloV7.Module.Marsoau.NetCord.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Descriptions;

public class UserDescription : DescriptionAttribute {
    public UserDescription() : base("user", "Query for a single user") { }
}

public class UsersDescription : DescriptionAttribute {
    public UsersDescription() : base("users", "Query for many users") { }
}
