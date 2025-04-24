using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Model
{
    public class SavedServer
    {
        public string Name;
        public IPEndPoint Host;

        public readonly List<Guid> Tokens;

        public SavedServer(string name, IPEndPoint host) {
            Name = name;
            Host = host;

            Tokens = new List<Guid>();
        }
    }
}
