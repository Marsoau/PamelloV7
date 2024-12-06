using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloEpisodeDTO : IPamelloDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Start { get; set; }
        public bool Skip { get; set; }
        public int SongId { get; set; }
    }
}
