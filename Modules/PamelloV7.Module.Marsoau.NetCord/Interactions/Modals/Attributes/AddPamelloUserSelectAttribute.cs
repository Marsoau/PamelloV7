using Microsoft.Extensions.DependencyInjection;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

public class AddPamelloUserSelectAttribute : AddSelectAttribute<IPamelloUser>
{
    public AddPamelloUserSelectAttribute(string property, string label) : base(property, label) { }

    public override StringMenuProperties AddPropertiesTo(DiscordModalBuilder builder, object? parentProperties) {
        var properties = base.AddPropertiesTo(builder, parentProperties);

        var users = Actions.Services.GetRequiredService<IPamelloUserRepository>();

        properties.Options = users.GetAll(Actions.ScopeUser).Select(user =>
            new StringMenuSelectOptionProperties(user.Name, user.Id.ToString())
        );
        
        return properties;
    }
}
