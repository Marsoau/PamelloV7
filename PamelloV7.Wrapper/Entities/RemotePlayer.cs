using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Wrapper.Entities.Attributes;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Entities;

[RemoteEntity<PamelloPlayerDto>("players", "IPamelloPlayer")]
public partial class RemotePlayer;
