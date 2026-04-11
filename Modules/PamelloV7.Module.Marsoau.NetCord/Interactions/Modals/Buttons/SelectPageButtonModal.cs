using Microsoft.Extensions.DependencyInjection;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Buttons;

[DiscordModal("Select page")]

[AddSelect<int>("Page*", "Page")]

public partial class SelectPageButtonModal
{
    public partial class Builder
    {
        public void Build(int page, int pageSize, int items) {
            var totalPages = items / pageSize + (items % pageSize > 0 ? 1 : 0);
            if (totalPages == 0) totalPages = 1;
            
            for (var i = 0; i < totalPages; i++) {
                var lastItem = (i + 1) * pageSize;
                if (lastItem > items) lastItem = items;
                
                Page.AddOptions(
                    new StringMenuSelectOptionProperties($"{i + 1} ({i * pageSize + 1} - {lastItem})", i.ToString())
                        .WithDefault(i == page)
                );
            }
        }
    }
    public async Task Submit() {
        var messages = Services.GetRequiredService<UpdatableMessageService>();
        var message = messages.Get(CallSite.Differentiator);
        
        if (message is not UpdatablePageMessage pageMessage) return;
        
        await pageMessage.SetPage(Page);
    }
}
