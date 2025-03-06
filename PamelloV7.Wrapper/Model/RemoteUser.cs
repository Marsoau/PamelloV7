using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Model
{
    public class RemoteUser : RemoteEntity<PamelloUserDTO>
    {
        public RemoteUser(PamelloUserDTO userDTO, PamelloClient client) : base(userDTO, client) {

        }
    }
}
