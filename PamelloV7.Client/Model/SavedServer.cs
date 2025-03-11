using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Model
{
    public class SavedServer
    {
        public string Name;
        public string Host;

        public readonly List<Guid> Tokens;

        public SavedServer(string name, string host) {
            Name = name;
            Host = host;

            Tokens = new List<Guid>();
        }
    }
}
