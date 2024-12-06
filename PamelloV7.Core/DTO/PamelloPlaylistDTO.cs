using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloPlaylistDTO : IPamelloDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AddedById { get; set; }
        public bool IsProtected { get; set; }

        public IEnumerable<int> SongsIds { get; set; }

        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}
