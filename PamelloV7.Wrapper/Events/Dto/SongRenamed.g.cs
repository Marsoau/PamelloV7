//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.SongRenamed")]
public class SongRenamed : SongNameUpdated
{
    public System.String OldName { get; set; }

}