using System.Text;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Logging.Messages;

public class UpdatableLogMessage : RefreshableLogMessage
{
    public readonly Func<string> GetContent;
    
    public UpdatableLogMessage(Func<string> getContent) {
        GetContent = getContent;
        
        ContentBuilder = new StringBuilder(GetContent());
    }

    public override void Refresh() {
        ContentBuilder.Clear().Append(GetContent());
        base.Refresh();
    }
}
