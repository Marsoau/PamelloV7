using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Wrapper.Entities.Attributes;
using PamelloV7.Wrapper.Entities.Base;
using PamelloV7.Wrapper.Entities.Dto;

namespace PamelloV7.Wrapper.Entities;

[RemoteEntity<RemotePlayerDto>("players", "IPamelloPlayer")]
public partial class RemotePlayer;
