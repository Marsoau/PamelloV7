using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PamelloV7.Core.Dto;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Dto.Other;

namespace PamelloV7.Framework.Dto
{
    public class HistoryRecordDto : PamelloEntityDto
    {
        public required int? PerformerId { get; set; }
        public required NestedEventDto Nested { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
